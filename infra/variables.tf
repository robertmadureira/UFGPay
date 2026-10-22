variable "project_id" {
  description = "ID do Projeto no Google Cloud (ex: ufgpay-production)"
  type        = string
}

variable "region" {
  description = "Região principal do GCP para provisionamento (ex: southamerica-east1 em São Paulo)"
  type        = string
  default     = "southamerica-east1"
}

variable "zone" {
  description = "Zona padrão dentro da região escolhida"
  type        = string
  default     = "southamerica-east1-a"
}

variable "environment" {
  description = "Identificador do ambiente (dev, staging, prod)"
  type        = string
  default     = "prod"
}

# -----------------------------------------------------------------------------
# Networking
# -----------------------------------------------------------------------------
variable "vpc_subnet_cidr" {
  description = "Faixa CIDR principal da subnet da aplicação na VPC"
  type        = string
  default     = "10.10.0.0/20"
}

variable "pods_subnet_cidr" {
  description = "Faixa CIDR secundária para Pods do GKE"
  type        = string
  default     = "10.20.0.0/16"
}

variable "services_subnet_cidr" {
  description = "Faixa CIDR secundária para Services do GKE"
  type        = string
  default     = "10.30.0.0/20"
}

# -----------------------------------------------------------------------------
# Cloud SQL (PostgreSQL 17)
# -----------------------------------------------------------------------------
variable "db_tier" {
  description = "Tier de máquina da instância Cloud SQL (ex: db-custom-2-7680, db-f1-micro)"
  type        = string
  default     = "db-custom-2-7680"
}

variable "db_edition" {
  description = "Edição do Cloud SQL (ENTERPRISE ou ENTERPRISE_PLUS)"
  type        = string
  default     = "ENTERPRISE"
}

variable "db_availability_type" {
  description = "Disponibilidade da instância Cloud SQL (ZONAL ou REGIONAL para alta disponibilidade)"
  type        = string
  default     = "ZONAL"
}

variable "db_version" {
  description = "Versão do banco de dados PostgreSQL no Cloud SQL"
  type        = string
  default     = "POSTGRES_17"
}

variable "db_disk_size_gb" {
  description = "Tamanho inicial do disco do Cloud SQL em GB"
  type        = number
  default     = 20
}

variable "db_deletion_protection" {
  description = "Protege a instância de Cloud SQL contra exclusão acidental via Terraform"
  type        = bool
  default     = true
}

# -----------------------------------------------------------------------------
# Memorystore for Redis
# -----------------------------------------------------------------------------
variable "redis_memory_size_gb" {
  description = "Capacidade de memória da instância Redis em GB"
  type        = number
  default     = 1
}

variable "redis_tier" {
  description = "Nível de serviço do Memorystore for Redis (BASIC ou STANDARD_HA)"
  type        = string
  default     = "BASIC"
}

# -----------------------------------------------------------------------------
# Google Kubernetes Engine (GKE)
# -----------------------------------------------------------------------------
variable "enable_gke_autopilot" {
  description = "Se verdadeiro, provisiona o cluster GKE em modo Autopilot (recomendado pelo Google Cloud)"
  type        = bool
  default     = true
}
