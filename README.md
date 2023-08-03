# Signals

The *signals* domain deals with GPS signals related to trucks.
Its purpose is to _aggregate_ signals from GPS devices representing
the location of trucks doing transports, and to _forward_ these
signals to customers who want to know where their trucks, or the 
trucks of their business partners, are located.

## Bounded Contexts

The signals domain consists of the following bounded contexts (BCs):

- [`aggregation`](./aggregation)
- [`forwarding`](./forwarding)

The deployment and management of infrastructure works the same
for each BC.

### Terraform

Infrastructure is managed via Terraform. Infrastructure changes
are automatically applied during deployments.

To connect to the terraform state from a development machine,
do the following in the BC's `terraform` folder:

```bash
gcloud auth application-default login
terraform init -backend-config=backend-config.hcl
terraform plan -out plan.tfplan
terraform apply plan.tfplan
```