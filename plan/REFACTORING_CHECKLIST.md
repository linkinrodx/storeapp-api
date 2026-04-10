# 📋 CHECKLIST: Refactorización Clean Code

## 🎯 FASE 1: Estructura & Constants (30 min)

### Carpetas
- [ ] Crear `src/Constants/`
- [ ] Crear `src/Services/Interfaces/`
- [ ] Crear `src/Services/Implementations/`
- [ ] Crear `src/Services/Validation/`
- [ ] Crear `src/Controllers/Base/`
- [ ] Crear `src/Extensions/`
- [ ] Crear `src/Repositories/Implementations/`

### Archivos - Constants
- [ ] `src/Constants/ErrorMessages.cs`
- [ ] `src/Constants/ValidationMessages.cs`
- [ ] `src/Constants/EntityTypes.cs`
- [ ] `src/Constants/OrderStatus.cs`
- [ ] `src/Constants/ImageEntityTypes.cs`

### Archivos - Base Classes
- [ ] `src/Services/Interfaces/IBaseService.cs`
- [ ] `src/Services/Implementations/BaseService.cs`
- [ ] `src/Controllers/Base/BaseController.cs`
- [ ] `src/Extensions/ServiceExtensions.cs`

### Build
- [ ] `dotnet build` → 0 errores ✅

---

## 🎯 FASE 2: Separar Controllers (45 min)

### Controllers Separados
- [ ] `src/Controllers/BrandsController.cs`
- [ ] `src/Controllers/FamiliesController.cs`
- [ ] `src/Controllers/CategoriesController.cs`
- [ ] `src/Controllers/ProductsController.cs`
- [ ] `src/Controllers/ImagesController.cs`
- [ ] `src/Controllers/ProfilesController.cs`
- [ ] `src/Controllers/CartController.cs`
- [ ] `src/Controllers/OrdersController.cs`
- [ ] `src/Controllers/WishlistController.cs`
- [ ] `src/Controllers/HealthController.cs`

### Cambios
- [ ] Cada controller hereda de `BaseController`
- [ ] Inyección: `(IXxxService service)` en lugar de `(AppDbContext db)`
- [ ] Todos compilan sin errores ✅

### Limpieza
- [ ] Eliminar `src/Controllers/Controllers.cs`

### Build & Test
- [ ] `dotnet build` → 0 errores ✅
- [ ] `curl https://localhost:5001/api/brands` → 200 OK ✅

---

## 🎯 FASE 3: Service Layer (2-3 horas)

### Interfaces CRUD Básicas
- [ ] `src/Services/Interfaces/IBrandService.cs`
- [ ] `src/Services/Interfaces/IFamilyService.cs`
- [ ] `src/Services/Interfaces/ICategoryService.cs`
- [ ] `src/Services/Interfaces/IProductService.cs`
- [ ] `src/Services/Interfaces/IImageService.cs`
- [ ] `src/Services/Interfaces/IProfileService.cs`
- [ ] `src/Services/Interfaces/ICartService.cs`
- [ ] `src/Services/Interfaces/IOrderService.cs`
- [ ] `src/Services/Interfaces/IWishlistService.cs`

### Implementaciones
- [ ] `src/Services/Implementations/BrandService.cs`
  - [ ] Hereda: `BaseService<Brand>, IBrandService`
  - [ ] Método: `IsSlugUniqueAsync(slug, excludeId)`
  
- [ ] `src/Services/Implementations/FamilyService.cs`
  - [ ] Hereda: `BaseService<Family>, IFamilyService`
  - [ ] Método: `IsSlugUniqueAsync(slug, excludeId)`
  
- [ ] `src/Services/Implementations/CategoryService.cs`
  - [ ] Hereda: `BaseService<Category>, ICategoryService`
  - [ ] Método: `IsSlugUniqueAsync(slug, excludeId)`
  
- [ ] `src/Services/Implementations/ProductService.cs`
  - [ ] Hereda: `BaseService<Product>, IProductService`
  - [ ] Métodos: `GetAllAsync()` con filtros, `CreateAsync()`
  
- [ ] `src/Services/Implementations/ImageService.cs`
  - [ ] Hereda: `BaseService<Image>, IImageService`
  - [ ] Método: `ValidateEntityType(entityType)`
  
- [ ] `src/Services/Implementations/ProfileService.cs`
  - [ ] Hereda: `BaseService<UserProfile>, IProfileService`
  
- [ ] `src/Services/Implementations/CartService.cs`
  - [ ] Hereda: `BaseService<CartItem>, ICartService`
  - [ ] Método: `UpsertItemAsync(userId, productId, quantity)`
  
- [ ] `src/Services/Implementations/OrderService.cs`
  - [ ] Hereda: `BaseService<Order>, IOrderService`
  - [ ] Método: `UpdateStatusAsync(orderId, newStatus)`
  - [ ] Estados: pending, processing, shipped, delivered, cancelled
  
- [ ] `src/Services/Implementations/WishlistService.cs`
  - [ ] Hereda: `BaseService<Wishlist>, IWishlistService`

### DI Registration
- [ ] `ServiceExtensions.cs` - Registrar todos los servicios

### Build & Validation
- [ ] `dotnet build` → 0 errores ✅
- [ ] Controllers < 20 LOC por método ✅

---

## 🎯 FASE 4: Repository Pattern (45 min)

### Implementación
- [ ] `src/Repositories/Implementations/GenericRepository.cs`
  - [ ] Métodos: `GetAllAsync()`, `GetByIdAsync()`, `CreateAsync()`, `UpdateAsync()`, `DeleteAsync()`, `ExistsAsync()`
  - [ ] Usar `.AsNoTracking()` en reads

### Actualizar BaseService
- [ ] `BaseService.cs` cambia de `AppDbContext` → `IRepository<T>`
- [ ] Todos los métodos usan `_repository.XXXAsync()`

### DI Update
- [ ] `ServiceExtensions.cs`:
  ```csharp
  services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
  ```

### Verificación
- [ ] `dotnet build` → 0 errores ✅
- [ ] Cadena DI: Controller → Service → Repository → DbContext ✅

---

## 🎯 FASE 5: N+1 Query Fixes (30 min)

### ProductService
- [ ] ❌ REMOVER: `await db.Entry(entity).Reference(x => x.Brand).LoadAsync();`
- [ ] ❌ REMOVER: `await db.Entry(entity).Reference(x => x.Family).LoadAsync();`
- [ ] ❌ REMOVER: `await db.Entry(entity).Reference(x => x.Category).LoadAsync();`
- [ ] ✅ USAR: `.Include()` en GetByIdAsync()

### Auditar Todas las Queries
- [ ] Verificar `.Include()` donde sea necesario
- [ ] Verificar `.AsNoTracking()` en reads
- [ ] Usar `.Select()` si es posible

### Performance Test
- [ ] EF Core profiler: ≤ 2 queries/request ✅

---

## 🎯 FASE 6: DTOs & Mappings (15 min)

### Revisar MappingProfile
- [ ] Actualizar si es necesario (generalmente NO)
- [ ] Verificar que mappings siguen mismo patrón

### Build
- [ ] `dotnet build` → 0 errores ✅

---

## 🎯 FASE 7: Documentación & Testing (60 min)

### XML Documentation
- [ ] `IBaseService.cs` - Documentar métodos base
- [ ] `IBrandService.cs` - Documentar métodos específicos
- [ ] `BrandService.cs` - Documentar implementación
- [ ] (repetir para otros servicios)
- [ ] `BaseController.cs` - Documentar helpers

### Unit Tests (15-20 tests)
- [ ] `BrandServiceTests.cs`
  - [ ] `CreateAsync_WithValidData_CreatesEntity()`
  - [ ] `CreateAsync_WithDuplicateSlug_ThrowsException()`
  - [ ] `IsSlugUniqueAsync_WithExistingSlug_ReturnsFalse()`
  - [ ] `GetAllAsync_WithPaging_ReturnsPaged()`

- [ ] `ProductServiceTests.cs`
  - [ ] `GetAllAsync_WithFilters_FiltersCorrectly()`
  - [ ] `CreateAsync_LoadsRelationships()`

- [ ] `OrderServiceTests.cs`
  - [ ] `UpdateStatus_WithValidStatus_UpdatesOrder()`
  - [ ] `UpdateStatus_WithInvalidStatus_ThrowsException()`

- [ ] (otros servicios: tests básicos)

### Build & Run Tests
- [ ] `dotnet build` → 0 errores ✅
- [ ] `dotnet test` → Todos pasan ✅

---

## ✅ VERIFICACIÓN FINAL

### Build
- [ ] ✅ `dotnet build` → 0 errores
- [ ] ✅ `dotnet test` → Todos pasan
- [ ] ✅ Warnings aceptables (mínimo)

### Código
- [ ] ✅ Archivo `Controllers.cs` eliminado
- [ ] ✅ 10 controllers en archivos separados
- [ ] ✅ Cada controller < 20 LOC por método
- [ ] ✅ Toda lógica en Services
- [ ] ✅ No hay magic strings fuera de `Constants/`
- [ ] ✅ Repository inyectado en Services
- [ ] ✅ BaseService implementa CRUD

### Endpoints (Manual Testing)
- [ ] ✅ `GET /api/brands` → 200 OK + PagedResponse
- [ ] ✅ `POST /api/brands` (slug duplicado) → 409 Conflict
- [ ] ✅ `GET /api/products?brandId=X&familyId=Y` → Filtra
- [ ] ✅ `POST /api/orders` → 201 Created, sin N+1 queries
- [ ] ✅ `PATCH /api/orders/{id}/status` → 200 OK

### Métricas
- [ ] ✅ LOC Controllers: 900 → 150-200
- [ ] ✅ Duplicación: 30% → 5%
- [ ] ✅ N+1 Queries: 10+ → ≤2
- [ ] ✅ Testability: 🔴 → ✅
- [ ] ✅ Mantenibilidad: 🔴 → ✅

---

## 📊 RESUMEN PROGRESO

```
FASE 1: [                         ] 0%  ☐
FASE 2: [                         ] 0%  ☐
FASE 3: [                         ] 0%  ☐
FASE 4: [                         ] 0%  ☐
FASE 5: [                         ] 0%  ☐
FASE 6: [                         ] 0%  ☐
FASE 7: [                         ] 0%  ☐
────────────────────────────────────────────
TOTAL:  [·························] 0%  ☐

Tiempo estimado: 5-6 hours
Tiempo real: -- (en curso)
```

---

**Uso:** 
1. Abre este archivo mientras trabajas
2. Marca con ☑️ cada tarea completada
3. Usa `dotnet build` entre fases para validar

**Última actualización:** 09/04/2026 | **Status:** Listo para Fase 1 ✅
