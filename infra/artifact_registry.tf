# =============================================================================
# Google Artifact Registry (Repositório Docker Privado para Microsserviços)
# =============================================================================

resource "google_artifact_registry_repository" "docker_repo" {
  provider = google-beta

  location      = var.region
  repository_id = "${locals.name_prefix}-docker-repo"
  description   = "Repositorio Docker para imagens dos microsservicos e frontend do UFG Pay"
  format        = "DOCKER"
  project       = var.project_id

  docker_config {
    immutable_tags = false
  }

  depends_on = [google_project_service.apis["artifactregistry.googleapis.com"]]
}
