# =============================================================================
# Google Kubernetes Engine (GKE Autopilot com Nós Privados e Workload Identity)
# =============================================================================

resource "google_container_cluster" "primary" {
  name     = "${local.name_prefix}-gke-cluster"
  location = var.region
  project  = var.project_id

  # GKE Autopilot: gerenciamento automatizado de nós, binários e segurança pelo Google
  enable_autopilot = var.enable_gke_autopilot

  network    = google_compute_network.vpc.id
  subnetwork = google_compute_subnetwork.subnet.id

  # Configuração de IPs secundários da VPC para Pods e Services
  ip_allocation_policy {
    cluster_secondary_range_name  = "gke-pods"
    services_secondary_range_name = "gke-services"
  }

  # Nós privados: nenhum worker node exposto à internet pública
  private_cluster_config {
    enable_private_nodes    = true
    enable_private_endpoint = false # Endpoint de controle acessível via kubectl autenticado
  }

  release_channel {
    channel = "REGULAR"
  }

  maintenance_policy {
    daily_maintenance_window {
      start_time = "05:00"
    }
  }

  depends_on = [
    google_project_service.apis["container.googleapis.com"],
    google_compute_subnetwork.subnet,
    google_compute_router_nat.nat
  ]
}
