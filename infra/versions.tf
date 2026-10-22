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

  # Descomente e configure para utilizar backend remoto no Cloud Storage (GCS)
  # backend "gcs" {
  #   bucket = "ufgpay-terraform-state"
  #   prefix = "environments/prod"
  # }
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
