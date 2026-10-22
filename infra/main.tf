# =============================================================================
# Habilitação Automática das APIs do Google Cloud Platform
# =============================================================================

locals {
  name_prefix = "ufgpay-${var.environment}"

  services_to_enable = [
    "compute.googleapis.com",           # VPC, Cloud Router, Cloud NAT
    "container.googleapis.com",         # GKE (Google Kubernetes Engine)
    "sqladmin.googleapis.com",          # Cloud SQL Admin API
    "redis.googleapis.com",             # Google Cloud Memorystore for Redis
    "servicenetworking.googleapis.com", # Private Services Access (VPC Peering)
    "artifactregistry.googleapis.com",  # Docker Registry
    "secretmanager.googleapis.com",     # Secret Manager (senhas e JWT)
    "monitoring.googleapis.com",        # Cloud Monitoring
    "logging.googleapis.com"            # Cloud Logging
  ]
}

resource "google_project_service" "apis" {
  for_each = toset(local.services_to_enable)

  project            = var.project_id
  service            = each.key
  disable_on_destroy = false
}
