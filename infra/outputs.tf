# =============================================================================
# Outputs da Infraestrutura (UFG Pay no Google Cloud Platform)
# =============================================================================

output "gcp_project_id" {
  description = "ID do Projeto GCP provisionado"
  value       = var.project_id
}

output "gcp_region" {
  description = "Região utilizada no GCP"
  value       = var.region
}

# -----------------------------------------------------------------------------
# Rede
# -----------------------------------------------------------------------------
output "vpc_name" {
  description = "Nome da VPC criada"
  value       = google_compute_network.vpc.name
}

output "subnet_name" {
  description = "Nome da Subnetwork regional"
  value       = google_compute_subnetwork.subnet.name
}

# -----------------------------------------------------------------------------
# Google Artifact Registry (Docker)
# -----------------------------------------------------------------------------
output "artifact_registry_repo" {
  description = "URL do Repositório Docker no Google Artifact Registry"
  value       = "${var.region}-docker.pkg.dev/${var.project_id}/${google_artifact_registry_repository.docker_repo.name}"
}

# -----------------------------------------------------------------------------
# Cloud SQL (PostgreSQL 17)
# -----------------------------------------------------------------------------
output "cloud_sql_instance_name" {
  description = "Nome da instância do Cloud SQL"
  value       = google_sql_database_instance.postgres.name
}

output "cloud_sql_connection_name" {
  description = "Connection Name do Cloud SQL para uso com Cloud SQL Auth Proxy"
  value       = google_sql_database_instance.postgres.connection_name
}

output "cloud_sql_private_ip" {
  description = "Endereço IP Privado da instância do Cloud SQL PostgreSQL"
  value       = google_sql_database_instance.postgres.private_ip_address
}

output "cloud_sql_databases" {
  description = "Bases de dados relacionais isoladas criadas"
  value = {
    user_db   = google_sql_database.user_db.name
    wallet_db = google_sql_database.wallet_db.name
    campus_db = google_sql_database.campus_db.name
  }
}

output "cloud_sql_user" {
  description = "Usuário principal da aplicação no banco de dados"
  value       = google_sql_user.app_user.name
}

# -----------------------------------------------------------------------------
# Google Cloud Memorystore for Redis
# -----------------------------------------------------------------------------
output "redis_host" {
  description = "IP Privado do cluster Redis Memorystore"
  value       = google_redis_instance.cache.host
}

output "redis_port" {
  description = "Porta do serviço Redis"
  value       = google_redis_instance.cache.port
}

# -----------------------------------------------------------------------------
# Google Kubernetes Engine (GKE)
# -----------------------------------------------------------------------------
output "gke_cluster_name" {
  description = "Nome do Cluster GKE"
  value       = google_container_cluster.primary.name
}

output "gke_cluster_endpoint" {
  description = "Endpoint de controle da API do GKE"
  value       = google_container_cluster.primary.endpoint
}

output "gke_get_credentials_command" {
  description = "Comando para autenticar o kubectl no cluster provisionado"
  value       = "gcloud container clusters get-credentials ${google_container_cluster.primary.name} --region ${var.region} --project ${var.project_id}"
}

# -----------------------------------------------------------------------------
# Secret Manager
# -----------------------------------------------------------------------------
output "secret_db_password_id" {
  description = "ID do segredo com a senha do banco no Secret Manager"
  value       = google_secret_manager_secret.db_password.secret_id
}

output "secret_jwt_id" {
  description = "ID do segredo com a chave de assinatura JWT no Secret Manager"
  value       = google_secret_manager_secret.jwt_secret.secret_id
}

# -----------------------------------------------------------------------------
# Frontend SPA & Cloud CDN (Equivalente AWS S3 + CloudFront)
# -----------------------------------------------------------------------------
output "frontend_bucket_name" {
  description = "Nome do Bucket GCS onde os arquivos estáticos do Angular devem ser publicados"
  value       = google_storage_bucket.frontend.name
}

output "frontend_cdn_ip" {
  description = "Endereço IP Público Global Anycast da Cloud CDN do Frontend"
  value       = google_compute_global_address.frontend_ip.address
}

output "frontend_url" {
  description = "URL pública de acesso ao Frontend via Cloud CDN"
  value       = "http://${google_compute_global_address.frontend_ip.address}"
}
