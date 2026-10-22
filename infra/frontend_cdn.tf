# =============================================================================
# Frontend SPA - Google Cloud Storage + Cloud CDN + Global Load Balancer
# (Equivalente oficial no Google Cloud ao S3 + CloudFront da AWS)
# =============================================================================

# -----------------------------------------------------------------------------
# 1. Bucket no Cloud Storage para os Arquivos Estáticos do Angular
# -----------------------------------------------------------------------------
resource "google_storage_bucket" "frontend" {
  name          = "${locals.name_prefix}-frontend-static-${var.project_id}"
  location      = var.region
  force_destroy = true
  project       = var.project_id

  uniform_bucket_level_access = true

  # Roteamento SPA (Single Page Application)
  # Qualquer rota do Angular (ex: /login, /campus) que não seja um arquivo estático
  # é respondida pelo index.html para o roteador do Angular assumir o controle.
  website {
    main_page_suffix = "index.html"
    not_found_page   = "index.html"
  }

  cors {
    origin          = ["*"]
    method          = ["GET", "HEAD", "OPTIONS"]
    response_header = ["*"]
    max_age_seconds = 3600
  }

  depends_on = [google_project_service.apis["compute.googleapis.com"]]
}

# Permissão de leitura pública para o conteúdo do bucket estático
resource "google_storage_bucket_iam_member" "public_read" {
  bucket = google_storage_bucket.frontend.name
  role   = "roles/storage.objectViewer"
  member = "allUsers"
}

# -----------------------------------------------------------------------------
# 2. Backend Bucket com Google Cloud CDN Habilitado (Borda Global Anycast)
# -----------------------------------------------------------------------------
resource "google_compute_backend_bucket" "frontend_cdn" {
  name        = "${locals.name_prefix}-frontend-backend-bucket"
  bucket_name = google_storage_bucket.frontend.name
  enable_cdn  = true
  project     = var.project_id

  cdn_policy {
    cache_mode        = "CACHE_ALL_STATIC"
    default_ttl       = 3600 # 1 hora de cache padrão nos pontos de presença mundiais da Google
    client_ttl        = 3600
    max_ttl           = 86400 # 24 horas máximo
    negative_caching  = true  # Cache de 404/erros por curto tempo
    serve_while_stale = 86400
  }
}

# -----------------------------------------------------------------------------
# 3. Mapeamento de Rotas (URL Map) do Load Balancer
# -----------------------------------------------------------------------------
resource "google_compute_url_map" "frontend_url_map" {
  name            = "${locals.name_prefix}-frontend-url-map"
  default_service = google_compute_backend_bucket.frontend_cdn.id
  project         = var.project_id
}

# -----------------------------------------------------------------------------
# 4. Endereço IP Público Global Anycast (Distribuição Global de Borda)
# -----------------------------------------------------------------------------
resource "google_compute_global_address" "frontend_ip" {
  name    = "${locals.name_prefix}-frontend-global-ip"
  project = var.project_id
}

# -----------------------------------------------------------------------------
# 5. Target HTTP Proxy e Forwarding Rule (Porta 80)
# -----------------------------------------------------------------------------
resource "google_compute_target_http_proxy" "frontend_http_proxy" {
  name    = "${locals.name_prefix}-frontend-http-proxy"
  url_map = google_compute_url_map.frontend_url_map.id
  project = var.project_id
}

resource "google_compute_global_forwarding_rule" "frontend_http_rule" {
  name                  = "${locals.name_prefix}-frontend-http-forwarding-rule"
  ip_protocol           = "TCP"
  port_range            = "80"
  target                = google_compute_target_http_proxy.frontend_http_proxy.id
  ip_address            = google_compute_global_address.frontend_ip.id
  load_balancing_scheme = "EXTERNAL"
  project               = var.project_id
}
