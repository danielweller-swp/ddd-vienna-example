# Using Terraform

We have one dedicated Terraform state per Bounded Context, stored in a (domain shared) blob storage.

## Local Development

For local development run the following commands:

Initialize your local Terraform environment

    terraform init -backend-config=backend-config.hcl

Validate your configuration before `plan`/`apply` with

    terraform validate

Verify your configuration with `plan` (specify extra variables via `-var NAME=VALUE`)

    terraform plan

Applying Terraform modifications should only be done via the CI/CD pipeline!

Before pushing changes make sure that all Terraform files are formatted correctly via

    terraform fmt

## Importing Resources

When migrating to Terraform previously created resources need to be imported into the Terraform state.

See `import.sh` for resources that need to be imported prior to the next `terraform apply` execution.
