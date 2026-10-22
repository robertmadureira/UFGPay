# =============================================================================
# Contas de Serviço (Service Accounts) e IAM (Princípio do Menor Privilégio)
# =============================================================================

# Service Account para os Nós e Pods do GKE
resource "google_service_account" "gke_sa" {
  account_id   = "${local.name_prefix}-gke-sa"
  display_name = "Service Account para o cluster GKE do UFG Pay"
  project      = var.project_id
}

# Permissões do GKE para métricas e logs
resource "google_project_iam_member" "gke_log_writer" {
  project = var.project_id
  role    = "roles/logging.logWriter"
  member  = "serviceAccount:${google_service_account.gke_sa.email}"
}

resource "google_project_iam_member" "gke_metric_writer" {
  project = var.project_id
  role    = "roles/monitoring.metricWriter"
  member  = "serviceAccount:${google_service_account.gke_sa.email}"
}

resource "google_project_iam_member" "gke_monitoring_viewer" {
  project = var.project_id
  role    = "roles/monitoring.viewer"
  member  = "serviceAccount:${google_service_account.gke_sa.email}"
}

resource "google_project_iam_member" "gke_ar_reader" {
  project = var.project_id
  role    = "roles/artifactregistry.reader"
  member  = "serviceAccount:${google_service_account.gke_sa.email}"
}

# Service Account de Aplicação (Workload Identity para os Microsserviços)
resource "google_service_account" "app_sa" {
  account_id   = "${local.name_prefix}-app-sa"
  display_name = "Service Account dos Microsservicos UFG Pay"
  project      = var.project_id
}

# Permite que os microsserviços acessem o Cloud SQL
resource "google_project_iam_member" "app_cloudsql_client" {
  project = var.project_id
  role    = "roles/cloudsql.client"
  member  = "serviceAccount:${google_service_account.app_sa.email}"
}

# Permite leitura dos segredos pelo Secret Manager (senhas de banco e chave JWT)
resource "google_secret_manager_secret_iam_member" "db_password_accessor" {
  secret_id = google_secret_manager_secret.db_password.id
  role      = "roles/secretmanager.secretAccessor"
  member    = "serviceAccount:${google_service_account.app_sa.email}"
}

resource "google_secret_manager_secret_iam_member" "jwt_secret_accessor" {
  secret_id = google_secret_manager_secret.jwt_secret.id
  role      = "roles/secretmanager.secretAccessor"
  member    = "serviceAccount:${google_service_account.app_sa.email}"
}
