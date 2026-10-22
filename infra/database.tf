# =============================================================================
# Google Cloud SQL (PostgreSQL 17 - Arquitetura Database-per-Service)
# =============================================================================

resource "google_sql_database_instance" "postgres" {
  name             = "${local.name_prefix}-postgres17"
  database_version = var.db_version
  region           = var.region
  project          = var.project_id

  deletion_protection = var.db_deletion_protection

  settings {
    tier              = var.db_tier
    availability_type = var.environment == "prod" ? "REGIONAL" : "ZONAL"
    disk_size         = var.db_disk_size_gb
    disk_type         = "PD_SSD"
    disk_autoresize   = true

    ip_configuration {
      ipv4_enabled                                  = false # Totalmente isolado sem IP público
      private_network                               = google_compute_network.vpc.id
      enable_private_path_for_google_cloud_services = true
    }

    backup_configuration {
      enabled                        = true
      start_time                     = "04:00"
      point_in_time_recovery_enabled = true
      transaction_log_retention_days = 7
      backup_retention_settings {
        retained_backups = 7
      }
    }

    database_flags {
      name  = "timezone"
      value = "UTC"
    }

    database_flags {
      name  = "max_connections"
      value = "200"
    }

    insights_config {
      query_insights_enabled  = true
      record_application_tags = true
      record_client_address   = true
    }
  }

  depends_on = [
    google_service_networking_connection.private_vpc_connection,
    google_project_service.apis["sqladmin.googleapis.com"]
  ]
}

# -----------------------------------------------------------------------------
# Bancos de Dados Isolados (Padrão Database-per-Service)
# -----------------------------------------------------------------------------

# 1. Base do UserService (Identidade e Matrículas)
resource "google_sql_database" "user_db" {
  name     = "ufg_user_db"
  instance = google_sql_database_instance.postgres.name
  project  = var.project_id
}

# 2. Base do WalletService (Saldos em Centavos e Recargas PIX)
resource "google_sql_database" "wallet_db" {
  name     = "ufg_wallet_db"
  instance = google_sql_database_instance.postgres.name
  project  = var.project_id
}

# 3. Base do CampusService (Catálogo do RU, Lanchonetes e Vouchers)
resource "google_sql_database" "campus_db" {
  name     = "ufg_campus_db"
  instance = google_sql_database_instance.postgres.name
  project  = var.project_id
}

# -----------------------------------------------------------------------------
# Usuário da Aplicação
# -----------------------------------------------------------------------------
resource "google_sql_user" "app_user" {
  name     = "ufgpay_app"
  instance = google_sql_database_instance.postgres.name
  password = random_password.db_password.result
  project  = var.project_id
}
