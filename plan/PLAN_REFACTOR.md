# Plan: Refactorización Clean Code - StoreApp.Api

## TL;DR
El proyecto tiene **900+ LOC en un archivo** (Controllers.cs), **~30% duplicación**, sin Service Layer y violaciones de SOLID. Se refactorizará en **7 fases prioritarias** siguiendo Clean Code principles.

---

## Problemas Identificados 🔴

| Problema | Severidad | Impacto |
|----------|-----------|---------|
| Controllers.cs monolítico (900 LOC) | CRÍTICA | Inmantenible, todo en 1 archivo |
| ~30% duplicación de código | CRÍTICA | 150+ LOC repetidas (CRUD patterns) |
| DbContext inyectado directamente | CRÍTICA | No testeable, tight coupling |
| IRepository no implementada | CRÍTICA | Interface definida pero NO se usa |
| Sin Service Layer | CRÍTICA | Lógica de negocio en controllers |
| 15+ magic strings dispersos | ALTA | "El slug ya está en uso." ×6 |
| N+1 queries en ProductService | ALTA | 3 `.LoadAsync()` innecesarias |

---

## Distribución Actual de LOC

```
Program.cs ............................ 25 LOC
Startup.cs ............................ 55 LOC
Controllers.cs ........................ 900+ LOC ⚠️ TODO AQUÍ
Models/Entities.cs .................... 110 LOC
Data/AppDbContext.cs .................. 230 LOC
DTOs (Requests + Responses) ........... 85 LOC
Middleware/ExceptionMiddleware.cs ..... 20 LOC
Mappings/MappingProfile.cs ............ 60 LOC
Repositories/Interfaces/IRepository.cs  12 LOC (NO SE USA)
─────────────────────────────────────────────────
TOTAL ................................ 1,575+ LOC
```

---

## Estructura Objetivo

```
src/
├── Controllers/              # HTTP Layer - Thin, sin lógica
│   ├── Base/
│   │   └── BaseController.cs (métodos respuesta comunes)
│   ├── BrandsController.cs
│   ├── FamiliesController.cs
│   ├── CategoriesController.cs
│   ├── ProductsController.cs
│   ├── ImagesController.cs
│   ├── ProfilesController.cs
│   ├── CartController.cs
│   ├── OrdersController.cs
│   ├── WishlistController.cs
│   └── HealthController.cs
├── Services/                 # Business Logic Layer (NUEVO)
│   ├── Interfaces/
│   │   ├── IBaseService.cs
│   │   ├── IBrandService.cs
│   │   ├── IFamilyService.cs
│   │   ├── ICategoryService.cs
│   │   ├── IProductService.cs
│   │   ├── IImageService.cs
│   │   ├── IProfileService.cs
│   │   ├── ICartService.cs
│   │   ├── IOrderService.cs
│   │   └── IWishlistService.cs
│   ├── Implementations/
│   │   ├── BaseService.cs
│   │   ├── BrandService.cs
│   │   ├── FamilyService.cs
│   │   ├── CategoryService.cs
│   │   ├── ProductService.cs
│   │   ├── ImageService.cs
│   │   ├── ProfileService.cs
│   │   ├── CartService.cs
│   │   ├── OrderService.cs
│   │   └── WishlistService.cs
│   └── Validation/
│       └── EntityValidationService.cs
├── Repositories/             # Data Access Layer
│   ├── Interfaces/
│   │   └── IRepository.cs (ya existe)
│   └── Implementations/
│       └── GenericRepository.cs
├── Constants/                # Centralizados (NUEVO)
│   ├── ErrorMessages.cs
│   ├── ValidationMessages.cs
│   ├── EntityTypes.cs
│   ├── OrderStatus.cs
│   └── ImageEntityTypes.cs
├── Extensions/               # DI (NUEVO)
│   └── ServiceExtensions.cs
├── Models/
│   └── Entities.cs (sin cambios)
├── Data/
│   └── AppDbContext.cs (sin cambios)
├── DTOs/
│   ├── Requests/
│   └── Responses/
├── Mappings/
│   └── MappingProfile.cs (actualizar si es necesario)
└── Middleware/
    └── ExceptionMiddleware.cs (sin cambios)
```

---

## Fases de Implementación

### **FASE 1: Estructura & Constants (30 min)**
**Dependencias:** Ninguna | **Paralelo:** Sí

**Tareas:**
- [ ] Crear carpeta `src/Constants/`
  - [ ] `ErrorMessages.cs` - Mensajes de error centralizados
  - [ ] `ValidationMessages.cs` - Mensajes de validación
  - [ ] `EntityTypes.cs` - Constantes: PRODUCT, FAMILY, CATEGORY, BRAND
  - [ ] `OrderStatus.cs` - Estados: PENDING, PROCESSING, SHIPPED, DELIVERED, CANCELLED
  - [ ] `ImageEntityTypes.cs` - Tipos de imagen

- [ ] Crear carpeta `src/Services/Base/`
  - [ ] `IBaseService.cs` - Interfaz base genérica CRUD
  - [ ] `BaseService.cs` - Clase base genérica reutilizable

- [ ] Crear `src/Controllers/Base/`
  - [ ] `BaseController.cs` - Métodos helpers de respuesta

- [ ] Crear `src/Extensions/`
  - [ ] `ServiceExtensions.cs` - Registros de Dependency Injection

**Verificación:**
```bash
dotnet build  # 0 errores
```

☑️ Proyecto compila, estructura lista.

---

### **FASE 2: Separar Controllers (45 min)**
**Dependencias:** Fase 1 | **Paralelo:** Sí (máximo 3 archivos simultáneamente)

**Tareas:**
- [ ] Crear archivos individuales en `src/Controllers/`:
  - [ ] `BrandsController.cs`
  - [ ] `FamiliesController.cs`
  - [ ] `CategoriesController.cs`
  - [ ] `ProductsController.cs`
  - [ ] `ImagesController.cs`
  - [ ] `ProfilesController.cs`
  - [ ] `CartController.cs`
  - [ ] `OrdersController.cs`
  - [ ] `WishlistController.cs`
  - [ ] `HealthController.cs`

- [ ] Cambios en cada controller:
  - [ ] Renombrar inyección: `(AppDbContext db, IMapper mapper)` → `(IBrandService service, IMapper mapper)`
  - [ ] Heredar de `BaseController`
  - [ ] Referencias a `db.Brands` → `await _service.GetAllAsync(...)`

- [ ] Eliminar `Controllers.cs` original

**Verificación:**
```bash
dotnet build  # 0 errores
curl https://localhost:5001/api/brands  # 200 OK
```

☑️ Rutas funcionan igual, no cambio de comportamiento.

---

### **FASE 3: Service Layer (2-3 horas)**
**Dependencias:** Fase 1 | **Paralelo:** Sí (2-3 servicios simultáneamente)

**Tareas para CADA SERVICIO (ejemplo: BrandService):**

1. **Crear interfaz:**
   ```
   src/Services/Interfaces/IBrandService.cs
   ```
   - Hereda de `IBaseService<Brand>`
   - Métodos específicos: `IsSlugUniqueAsync(slug, excludeId)`
   - Métodos CRUD base heredados

2. **Crear implementación:**
   ```
   src/Services/Implementations/BrandService.cs
   ```
   - Hereda de `BaseService<Brand>, IBrandService`
   - Inyecta: `IRepository<Brand>, IMapper`
   - Implementa lógica específica

3. **Mover lógica del controller:**
   - ✂️ Validación de slug → `IsSlugUniqueAsync()`
   - ✂️ Creación de entidad → `CreateAsync(request)`
   - ✂️ Actualización → `UpdateAsync(id, request)`
   - ✂️ Soft delete → `DeleteAsync(id)`
   - ✂️ Queries con filtros → `GetAllAsync(page, pageSize, onlyActive)`

4. **Registrar en `ServiceExtensions.cs`:**
   ```csharp
   services.AddScoped<IBrandService, BrandService>();
   ```

**Aplicar a:**
- [ ] BrandService
- [ ] FamilyService
- [ ] CategoryService
- [ ] ProductService (especial: N+1 queries)
- [ ] ImageService
- [ ] ProfileService
- [ ] CartService (lógica especial: upsert)
- [ ] OrderService (lógica especial: estados)
- [ ] WishlistService

**Verificación:**
```bash
dotnet build  # 0 errores
# Tests unitarios pasan (futura Fase 7)
curl -X POST https://localhost:5001/api/brands \
  -H "Content-Type: application/json" \
  -d '{"name":"Test","slug":"test"}'  # 201 Created
```

☑️ Servicios creados, controllers simplificados.

---

### **FASE 4: Repository Pattern (45 min)**
**Dependencias:** Fase 1 | **Paralelo:** No (después de serviceextensions)

**Tareas:**

1. **Crear `src/Repositories/Implementations/GenericRepository.cs`:**
   ```csharp
   public class GenericRepository<T> : IRepository<T> where T : class
   {
       private readonly AppDbContext _context;
       
       public GenericRepository(AppDbContext context) => _context = context;
       
       public async Task<IEnumerable<T>> GetAllAsync(int page, int pageSize, bool onlyActive)
           => await _context.Set<T>()
               .Skip((page - 1) * pageSize)
               .Take(pageSize)
               .AsNoTracking()
               .ToListAsync();
       
       // Implementar resto: GetByIdAsync, CreateAsync, UpdateAsync, DeleteAsync, ExistsAsync
   }
   ```

2. **Actualizar `BaseService.cs`:**
   - ✂️ Cambiar: `private AppDbContext _context;` → `private IRepository<T> _repository;`
   - ✂️ Cambiar queries LINQ directo → llamadas a `_repository.XXXAsync()`

3. **Registrar en `ServiceExtensions.cs`:**
   ```csharp
   services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
   ```

4. **Verificar que BaseService.cs usa Repository:**
   - [ ] `_repository.GetAllAsync()` en lugar de `_context.Set<T>()`
   - [ ] `_repository.CreateAsync(entity)` en lugar de `_context.Set<T>().Add(entity)`
   - [ ] `_repository.ExistsAsync(predicate)` para validaciones

**Verificación:**
```bash
dotnet build  # 0 errores
```

Cadena DI: Controller → Service → Repository → DbContext ✅

---

### **FASE 5: Corregir N+1 Queries (30 min)**
**Dependencias:** Fase 3 | **Paralelo:** No

**Problemas a corregir:**

1. **ProductService.CreateAsync() - 3 queries innecesarias:**
   ```csharp
   // ❌ ANTES: 3 queries separadas
   await db.Entry(entity).Reference(x => x.Brand).LoadAsync();
   await db.Entry(entity).Reference(x => x.Family).LoadAsync();
   await db.Entry(entity).Reference(x => x.Category).LoadAsync();
   
   // ✅ DESPUÉS: Usar Include() en consulta inicial
   // O no cargar si no se necesitan
   ```

2. **Auditar todas las queries públicas:**
   - [ ] ProductService.GetAllAsync() - Verificar `.Include()` correcto
   - [ ] OrderService.GetByIdAsync() - Usa `.ThenInclude()` bien ✅
   - [ ] Agregar `.AsNoTracking()` en queries de lectura
   - [ ] Usar `.Select()` para projection cuando sea posible

**Verificación:**
```bash
# EF Core profiler: ≤ 2 queries por request
# (1 principal + posible 1 de count)
```

---

### **FASE 6: DTOs y Mappings (15 min)**
**Dependencias:** Fase 3 | **Paralelo:** Sí

**Tareas:**
- [ ] Revisar `MappingProfile.cs`
- [ ] Agregar mapeos para nuevas DTOs si es necesario (generalmente NO)
- [ ] Verificar que mapeos siguen mismo patrón

**Verificación:**
```bash
dotnet build  # 0 errores
```

Mappings exactamente iguales, solo reorganizados.

---

### **FASE 7: Documentación y Testing (60 min)**
**Dependencias:** Todas las fases | **Paralelo:** No

**Tareas:**

1. **Agregar XML Comments:**
   - [ ] `IBaseService.cs` - Documentar métodos genéricos
   - [ ] `IBrandService.cs`, etc. - Métodos específicos
   - [ ] `BaseController.cs` - Helpers de respuesta
   - [ ] Métodos públicos en Services

2. **Crear tests unitarios (15-20 tests):**
   - [ ] `BrandService.CreateAsync_WithValidData_CreatesEntity()`
   - [ ] `BrandService.CreateAsync_WithDuplicateSlug_ThrowsException()`
   - [ ] `BrandService.IsSlugUniqueAsync_WithExistingSlug_ReturnsFalse()`
   - [ ] `ProductService.GetAllAsync_WithFilters_FiltersCorrectly()`
   - [ ] `OrderService.UpdateStatus_WithValidStatus_UpdatesOrder()`
   - [ ] etc.

**Verificación:**
```bash
dotnet test  # Todos pasan ✅
```

---

## Cambios Específicos en Archivos

### **Program.cs**
```diff
+ builder.Services.AddServices();
```

Agregar DESPUÉS de `builder.Services.AddControllers();`

---

## Archivos a CREAR

| Archivo | Carpeta | Propósito |
|---------|---------|----------|
| `ErrorMessages.cs` | `Constants/` | Magic strings centralizados |
| `ValidationMessages.cs` | `Constants/` | Mensajes de validación |
| `EntityTypes.cs` | `Constants/` | Tipos de entidad |
| `OrderStatus.cs` | `Constants/` | Estados de orden |
| `ImageEntityTypes.cs` | `Constants/` | Tipos de imagen |
| `IBaseService.cs` | `Services/Interfaces/` | Interfaz base CRUD |
| `BaseService.cs` | `Services/Implementations/` | Clase base CRUD genérica |
| `IBrandService.cs` | `Services/Interfaces/` | Interfaz específica |
| `BrandService.cs` | `Services/Implementations/` | Implementación |
| (×8 más servicios) | `Services/` | - |
| `BaseController.cs` | `Controllers/Base/` | Métodos helpers |
| `BrandsController.cs` | `Controllers/` | Separado |
| (×9 más controllers) | `Controllers/` | Separados |
| `GenericRepository.cs` | `Repositories/Implementations/` | Repositorio genérico |
| `ServiceExtensions.cs` | `Extensions/` | DI registration |

---

## Archivos a ELIMINAR

| Archivo | Razón |
|---------|-------|
| `src/Controllers/Controllers.cs` | Separado en 10 archivos |

---

## Archivos SIN CAMBIOS

| Archivo | Por qué |
|---------|--------|
| `Program.cs` | Solo agregar 1 línea de DI |
| `Startup.cs` | Configuración DbContext, OAuth, etc. |
| `AppDbContext.cs` | Configuración de modelos OK |
| `Models/Entities.cs` | Entidades correctas |
| `DTOs/Requests/*.cs` | Validaciones OK |
| `DTOs/Responses/*.cs` | Estructuras OK |
| `MappingProfile.cs` | Actualizaciones menores |
| `ExceptionMiddleware.cs` | Funcional ✓ |

---

## Resultados Post-Refactor

### Métricas

```
ANTES                          DESPUÉS
─────────────────────────────────────────
Controllers.cs: 900 LOC    →   10 files: 150 LOC
Duplicación: 30%           →   Duplicación: 5%
Service Layer: ❌          →   Service Layer: ✅
Repository Pattern: ❌     →   Repository Pattern: ✅
Testability: 🔴 Imposible  →   Testability: ✅ Alta
Mantenibilidad: 🔴 Baja    →   Mantenibilidad: ✅ Alta
```

### LOC Controller Promedio

```
ANTES (en 1 archivo):
GetAll()    → 15 LOC
GetById()   → 5 LOC
Create()    → 15 LOC
Update()    → 15 LOC
Delete()    → 5 LOC
─────────────────────
Total:      50 LOC/controller × 7 = 350 LOC

DESPUÉS (separados, con servicios):
GetAll()    → 2 LOC
GetById()   → 1 LOC
Create()    → 1 LOC
Update()    → 1 LOC
Delete()    → 1 LOC
─────────────────────
Total:      6 LOC/controller × 10 = 60 LOC
```

---

## Timeline Total: 5-6 horas

- **Fase 1 (Estructura & Constants):** 30 min ⏱️
- **Fase 2 (Separar Controllers):** 45 min ⏱️
- **Fase 3 (Service Layer):** 2-3 horas ⏱️ (puede ser paralelo)
- **Fase 4 (Repository Pattern):** 45 min ⏱️
- **Fase 5 (N+1 Queries):** 30 min ⏱️
- **Fase 6 (DTOs/Mappings):** 15 min ⏱️
- **Fase 7 (Documentación/Tests):** 60 min ⏱️
- **────────────────────────**
- **TOTAL:** ~5-6 horas de trabajo

---

## Decisiones de Diseño

### 1. ¿Por qué Service Layer?
- ✅ **Separación de concerns:** HTTP logic ≠ Business logic
- ✅ **Testability:** Servicios mockeables sin HttpContext
- ✅ **Reusability:** Servicios usables desde WebSockets, gRPC, etc.
- ✅ **Clean Code:** Controllers delgados (SRP)

### 2. ¿Por qué IBaseService genérica?
- ✅ **DRY:** CRUD estándar en clase base
- ✅ **Consistencia:** Interfaz uniforme
- ✅ **Escalabilidad:** Nuevos servicios heredan automáticamente

### 3. ¿Por qué dividir Controllers.cs?
- ✅ **Navegación:** VS Code Explorer limpio
- ✅ **Mantenimiento:** 1 archivo = 1 responsabilidad
- ✅ **Escalabilidad:** Fácil agregar nuevos controllers

### 4. ¿Por qué Constants vs Enums?
- ✅ **Ahora:** Constants suficientes, fácil serializar
- 🔮 **Futura:** Migrar a Enums si se necesita type-safety

### 5. ¿Qué NO cambiar?
- ✅ Estructura de DTOs (Records están bien)
- ✅ AppDbContext (configuración excelente)
- ✅ Middleware (ExceptionMiddleware funcional)

---

## Verificación Final

### Tests Manuales Críticos
```bash
# Test 1: GetAll sin filtros
curl https://localhost:5001/api/brands

# Test 2: GetAll con paginación
curl "https://localhost:5001/api/brands?page=1&pageSize=10"

# Test 3: Create con slug duplicado (409 Conflict)
curl -X POST https://localhost:5001/api/brands \
  -H "Content-Type: application/json" \
  -d '{"name":"Test","slug":"existing-slug"}'

# Test 4: GetAll products con filtros
curl "https://localhost:5001/api/products?brandId=X&familyId=Y&search=test"

# Test 5: Create order (sin N+1 queries)
curl -X POST https://localhost:5001/api/orders \
  -H "Content-Type: application/json" \
  -d '{"userId":"X","items":[...]}'
```

### Verificación de Código
- [ ] `dotnet build` → 0 errores
- [ ] `dotnet test` → Todos pasan
- [ ] Controllers.cs no existe
- [ ] Cada controller < 20 LOC por método
- [ ] Toda lógica de negocio en Services
- [ ] No hay magic strings fuera de `Constants/`
- [ ] Repository inyectado en Services (no AppDbContext)
- [ ] BaseService implementa CRUD estándar

### Checklist Métrico
- [ ] LOC Controllers: 900 → 150-200 ✅
- [ ] Duplicación: 30% → 5% ✅
- [ ] N+1 Queries: 10+ → ≤2 ✅
- [ ] Testability: 🔴 → ✅ ✅
- [ ] Mantenibilidad: 🔴 → ✅ ✅

---

## Consideraciones Futuras

### 🔮 Próximos Pasos (Out of Scope)
1. **FluentValidation** - Validaciones más complejas
2. **Specification Pattern** - Queries dinámicas
3. **CQRS** - Si complejidad creciente
4. **Integration Tests** - CON testcontainers
5. **Caching** - Redis para datos estáticos
6. **Rate Limiting** - Por endpoint

---

## Estado Actual

```
Iniciado: [✓]  04/09/2026
Fase 1:   [ ]  Por comenzar
Fase 2:   [ ]  Por comenzar
Fase 3:   [ ]  Por comenzar
Fase 4:   [ ]  Por comenzar
Fase 5:   [ ]  Por comenzar
Fase 6:   [ ]  Por comenzar
Fase 7:   [ ]  Por comenzar
────────────────────────
Completado: [·········]  0%
```

---

**Documento generado:** 09/04/2026 | **Versión:** 1.0 | **Status:** 📋 Listo para ejecutar
