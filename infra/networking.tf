# =============================================================================
# Rede VPC Customizada e Subnets
# =============================================================================

resource "google_compute_network" "vpc" {
  name                    = "${local.name_prefix}-vpc"
  auto_create_subnetworks = false
  routing_mode            = "REGIONAL"
  project                 = var.project_id

  depends_on = [google_project_service.apis["compute.googleapis.com"]]
}

resource "google_compute_subnetwork" "subnet" {
  name                     = "${local.name_prefix}-subnet-${var.region}"
  ip_cidr_range            = var.vpc_subnet_cidr
  region                   = var.region
  network                  = google_compute_network.vpc.id
  private_ip_google_access = true
  project                  = var.project_id

  secondary_ip_range {
    range_name    = "gke-pods"
    ip_cidr_range = var.pods_subnet_cidr
  }

  secondary_ip_range {
    range_name    = "gke-services"
    ip_cidr_range = var.services_subnet_cidr
  }
}

# =============================================================================
# Cloud Router e Cloud NAT (Acesso de Saída para Instâncias/Pods Privados)
# =============================================================================

resource "google_compute_router" "router" {
  name    = "${local.name_prefix}-nat-router"
  region  = var.region
  network = google_compute_network.vpc.id
  project = var.project_id
}

resource "google_compute_router_nat" "nat" {
  name                               = "${local.name_prefix}-nat-gateway"
  router                             = google_compute_router.router.name
  region                             = var.region
  nat_ip_allocate_option             = "AUTO_ONLY"
  source_subnetwork_ip_ranges_to_nat = "ALL_SUBNETWORKS_ALL_IP_RANGES"
  project                            = var.project_id

  log_config {
    enable = true
    filter = "ERRORS_ONLY"
  }
}

# =============================================================================
# Private Services Access (Peering Privado para Cloud SQL e Memorystore Redis)
# =============================================================================

resource "google_compute_global_address" "private_ip_alloc" {
  name          = "${local.name_prefix}-private-ip-alloc"
  purpose       = "VPC_PEERING"
  address_type  = "INTERNAL"
  prefix_length = 16
  network       = google_compute_network.vpc.id
  project       = var.project_id

  depends_on = [google_project_service.apis["servicenetworking.googleapis.com"]]
}

resource "google_service_networking_connection" "private_vpc_connection" {
  network                 = google_compute_network.vpc.id
  service                 = "servicenetworking.googleapis.com"
  reserved_peering_ranges = [google_compute_global_address.private_ip_alloc.name]

  depends_on = [google_compute_global_address.private_ip_alloc]
}

# =============================================================================
# Regras de Firewall
# =============================================================================

# Tráfego interno entre os pods, microsserviços e gateways da VPC
resource "google_compute_firewall" "allow_internal" {
  name        = "${local.name_prefix}-allow-internal"
  network     = google_compute_network.vpc.id
  project     = var.project_id
  description = "Permite comunicacao interna irrestrita entre recursos da VPC"

  allow {
    protocol = "tcp"
    ports    = ["0-65535"]
  }
  allow {
    protocol = "udp"
    ports    = ["0-65535"]
  }
  allow {
    protocol = "icmp"
  }

  source_ranges = [
    var.vpc_subnet_cidr,
    var.pods_subnet_cidr,
    var.services_subnet_cidr
  ]
}

# Health Checks do Google Cloud Load Balancer
resource "google_compute_firewall" "allow_health_checks" {
  name        = "${local.name_prefix}-allow-health-checks"
  network     = google_compute_network.vpc.id
  project     = var.project_id
  description = "Permite sondas de Health Check dos Load Balancers do Google Cloud"

  allow {
    protocol = "tcp"
  }

  source_ranges = [
    "35.191.0.0/16",
    "130.211.0.0/22"
  ]
}
