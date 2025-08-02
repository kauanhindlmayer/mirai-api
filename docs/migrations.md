# EF Migrations Quick Reference

## Create Migration

```bash
dotnet ef migrations add <MigrationName> --project src/Infrastructure --startup-project src/Presentation -o Persistence/Migrations
```

## Apply Migration

```bash
dotnet ef database update --project src/Infrastructure --startup-project src/Presentation
```

## Remove Last Migration

```bash
dotnet ef migrations remove --project src/Infrastructure --startup-project src/Presentation
```
