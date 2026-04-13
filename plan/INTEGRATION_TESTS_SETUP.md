# 📋 Setup Final - Integration Tests Separados

## ✅ Cambios Realizados

### 1. Nuevo Proyecto Creado
```
src/StoreApp.IntegrationTests/
├── Fixtures/
├── Endpoints/
├── IntegrationTestBase.cs
├── README.md
└── StoreApp.IntegrationTests.csproj
```

### 2. Archivos Base Incluidos
- ✅ `TestDatabaseManager.cs` - Gestiona PostgreSQL con TestContainers
- ✅ `StoreAppWebApplicationFactory.cs` - WebApplicationFactory personalizado
- ✅ `IntegrationTestBase.cs` - Clase base para todos los tests
- ✅ `BrandEndpointsTests.cs` - Tests de ejemplo (Brand)
- ✅ `ProductEndpointsTests.cs` - Tests de ejemplo (Product)

### 3. Workflows Actualizados
- ✅ `.github/workflows/deploy.yml` - Solo Unit Tests antes de deploy
- ✅ `.github/workflows/integration-tests.yml` - NEW: Tests post-deploy

---

## 🚀 Pasos para Completar Setup

### PASO 1: Agregar Secrets en GitHub

1. Ve a: **Settings → Secrets and variables → Actions**
2. Haz click en **"New repository secret"**
3. Agrega estos secrets:

```
Name: API_GATEWAY_URL
Value: https://ba3hjkli0e.execute-api.sa-east-1.amazonaws.com/prod

Name: AWS_ACCESS_KEY_ID
Value: [tu access key]

Name: AWS_SECRET_ACCESS_KEY
Value: [tu secret key]

Name: S3_BUCKET
Value: [nombre-de-tu-bucket]

Name: LAMBDA_FUNCTION_NAME
Value: storeapp-api-function
```

### PASO 2: Actualizar `.sln`

Si tienes un archivo `.sln`, agregar el nuevo proyecto:

```bash
dotnet sln src/StoreApp.Api.sln add src/StoreApp.IntegrationTests/StoreApp.IntegrationTests.csproj
```

O manualmente en Visual Studio:
- Right-click en Solution
- Add > Existing Project
- Seleccionar `src/StoreApp.IntegrationTests/StoreApp.IntegrationTests.csproj`

### PASO 3: Eliminar tests duplicados en StoreApp.Test (Opcional)

Si quieres mantener StoreApp.Test limpio de integration tests:

```bash
# Eliminar carpeta IntegrationTests de StoreApp.Test
rm -r src/StoreApp.Test/IntegrationTests/
rm -r src/StoreApp.Test/Fixtures/

# O en Windows:
rmdir /s src\StoreApp.Test\IntegrationTests
rmdir /s src\StoreApp.Test\Fixtures
```

> Nota: Mantener `Services/` en StoreApp.Test

### PASO 4: Commit y Push

```bash
git add .
git commit -m "refactor: separate integration tests into StoreApp.IntegrationTests project"
git push origin main
```

---

## 🧪 Verificar Setup

### Local

```bash
# Compilar proyecto
dotnet build src/StoreApp.IntegrationTests/StoreApp.IntegrationTests.csproj

# Ejecutar tests locales (con TestContainers)
dotnet test src/StoreApp.IntegrationTests/
```

### En GitHub

1. Ve a: **Actions**
2. Espera a que `Deploy to AWS Lambda` termine exitosamente
3. Verifica que `Integration Tests` se ejecute automáticamente después
4. Revisa los resultados

---

## 📊 Flujo Completo

```
┌─────────────────────────────────────────┐
│ git push origin main                    │
└────────────────┬────────────────────────┘
                 ↓
┌─────────────────────────────────────────┐
│ GitHub Actions: Deploy to AWS Lambda    │
├─────────────────────────────────────────┤
│ ✅ Build                                │
│ ✅ Unit Tests (Services)                │
│ ✅ Deploy to Lambda                     │
│ ✅ Verify                               │
└────────────────┬────────────────────────┘
                 ↓ (IF SUCCESS)
        ┌────────────────────┐
        │   Wait 15 seconds  │
        └────────────────────┘
                 ↓
┌─────────────────────────────────────────┐
│ GitHub Actions: Integration Tests       │
├─────────────────────────────────────────┤
│ ✅ Run Integration Tests (vs Lambda)    │
│                                         │
│ If ENABLE_ROLLBACK='true' && fails:     │
│   🔄 Rollback to previous version       │
│ Else:                                   │
│   ⚠️ Just notify                        │
└─────────────────────────────────────────┘
```

---

## 🎯 Configurar Rollback

### Opción A: Deshabilitar Rollback (Default)
```yaml
# Mantener en integration-tests.yml:
ENABLE_ROLLBACK: ${{ github.event.inputs.enable_rollback || 'false' }}
```
→ Fails no hacen rollback automático

### Opción B: Habilitar Rollback
```yaml
# Cambiar a true en integration-tests.yml:
ENABLE_ROLLBACK: 'true'
```
→ Cualquier fallo de tests hace rollback

### Opción C: Rollback Manual
En GitHub Actions:
1. **Integration Tests** → **Run workflow**
2. Seleccionar **`enable_rollback`** = `true`
3. Click **"Run workflow"**

---

## 📝 Próximos Pasos

1. **✅ Completar Setup** - Sigue los 4 pasos arriba
2. **Crear más tests** - Agrega tests para:
   - Categories
   - Families
   - Orders
   - Wishlist
   - Etc.
3. **Monitoreo** - Revisa GitHub Actions después de cada push
4. **Documentación** - Lee [IntegrationTests README](./README.md)

---

## ❓ FAQ

**P: ¿Los tests locales usarán TestContainers o Lambda?**
R: Locales usan TestContainers (PostgreSQL en contenedor). Post-deploy usan Lambda real.

**P: ¿Si un integration test falla, se revierte el código?**
R: Solo si `ENABLE_ROLLBACK='true'`. Por defecto, solo notifica.

**P: ¿Puedo ejecutar tests locales sin Docker?**
R: No, TestContainers requiere Docker.

**P: ¿Los tests unit y integration se ejecutan juntos?**
R: No. Unit tests en deploy, integration tests después de deploy exitoso.

**P: ¿Dónde veo los resultados de los tests?**
R: GitHub Actions → Integration Tests → Artifacts

---

## 🔗 Referencias

- [IntegrationTests README](./README.md)
- [Deploy Workflow](./.github/workflows/deploy.yml)
- [Integration Tests Workflow](./.github/workflows/integration-tests.yml)
- [GitHub Secrets Docs](https://docs.github.com/en/actions/security-guides/encrypted-secrets)

---

## ✨ ¡Listo!

El setup está completado. Ahora puedes:

1. Agregar más tests en `src/StoreApp.IntegrationTests/Endpoints/`
2. Hacer push a `main` para trigger automático de workflows
3. Monitorear en GitHub Actions
4. Configurar rollback según necesites

¿Alguna duda? Revisa el README del proyecto. 📚
