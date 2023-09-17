# Signals

The **signals** domain deals with GPS signals related to trucks.
Its purpose is to _aggregate_ signals from GPS devices representing
the location of trucks doing transports, and to _forward_ these
signals to customers who want to know where their trucks, or the 
trucks of their business partners, are located.

## Bounded Contexts

The signals domain consists of the following bounded contexts (BCs):

- [`aggregation`](./aggregation)
- [`forwarding`](./forwarding)
- [`notifications`](./notifications)

The deployment and management of infrastructure works the same
for each BC.

### Package Repository

Some of our BCs use a private NuGet registry hosted by GitHub.
To setup access to the registry, please make a copy of [`nuget.config.example`](nuget.config.example)
named `nuget.config` and replace USERNAME by your GitHub username and
TOKEN by a personal access token. See [the GitHub docs](https://docs.github.com/en/packages/working-with-a-github-packages-registry/working-with-the-nuget-registry#authenticating-with-a-personal-access-token)
for more information.

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