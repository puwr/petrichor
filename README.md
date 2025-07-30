# Full-stack learning project

**Backend**

- .NET
- Modular Monolith
- CQRS
- JWT authentication (HTTP-only cookies)
- Image upload

**Frontend**

- Angular
- Angular CDK
- Elf store

# How to run

```
dotnet ef database update -p .\src\Infrastructure\ -s .\src\API\
```

```
dotnet run --project .\src\API\
```

```
cd .\src\Frontend\
```

```
pnpm install
```

```
ng serve
```
