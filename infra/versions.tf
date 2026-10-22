terraform {
  required_version = ">= 1.5.0"

  required_providers {
    google = {
      source  = "hashicorp/google"
      version = "~> 6.0"
    }
    google-beta = {
      source  = "hashicorp/google-beta"
      version = "~> 6.0"
    }
    random = {
      source  = "hashicorp/random"
      version = "~> 3.6"
    }
  }

  # Backend remoto no Google Cloud Storage (GCS)
  # O bucket é configurado dinamicamente via -backend-config no CI/CD
  backend "gcs" {}
}

provider "google" {
  project = var.project_id
  region  = var.region
  zone    = var.zone

  default_labels = {
    project     = "ufgpay"
    managed_by  = "terraform"
    environment = var.environment
  }
}

provider "google-beta" {
  project = var.project_id
  region  = var.region
  zone    = var.zone

  default_labels = {
    project     = "ufgpay"
    managed_by  = "terraform"
    environment = var.environment
  }
}
