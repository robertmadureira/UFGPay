# =============================================================================
# Google Secret Manager e Geradores de Segredos Seguros
# =============================================================================

# Senha aleatória forte para o usuário do PostgreSQL
resource "random_password" "db_password" {
  length           = 32
  special          = true
  override_special = "!#$%&*()-_=+[]{}<>:?"
}

# Chave secreta de 64 caracteres para assinatura dos tokens JWT
resource "random_password" "jwt_secret" {
  length  = 64
  special = false
}

# -----------------------------------------------------------------------------
# Segredo: Senha do Banco PostgreSQL
# -----------------------------------------------------------------------------
resource "google_secret_manager_secret" "db_password" {
  secret_id = "${local.name_prefix}-db-password"
  project   = var.project_id

  replication {
    auto {}
  }

  depends_on = [google_project_service.apis["secretmanager.googleapis.com"]]
}

resource "google_secret_manager_secret_version" "db_password" {
  secret      = google_secret_manager_secret.db_password.id
  secret_data = random_password.db_password.result
}

# -----------------------------------------------------------------------------
# Segredo: Chave JWT
# -----------------------------------------------------------------------------
resource "google_secret_manager_secret" "jwt_secret" {
  secret_id = "${local.name_prefix}-jwt-secret"
  project   = var.project_id

  replication {
    auto {}
  }

  depends_on = [google_project_service.apis["secretmanager.googleapis.com"]]
}

resource "google_secret_manager_secret_version" "jwt_secret" {
  secret      = google_secret_manager_secret.jwt_secret.id
  secret_data = random_password.jwt_secret.result
}
