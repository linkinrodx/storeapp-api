# StoreApp API — .NET 10 + AWS Lambda + Supabase

API REST serverless para gestión de tienda de fragancias. Construida con **.NET 10**, **EF Core + Npgsql**, desplegable en **AWS Lambda** vía API Gateway.

---

## Requisitos previos

| Herramienta | Versión mínima |
|---|---|
| .NET SDK | 10.0 |
| AWS CLI | 2.x |
| Amazon.Lambda.Tools | última |

```bash
# Instalar herramienta Lambda de .NET
dotnet tool install -g Amazon.Lambda.Tools
```

---

## Configuración

### 1. Cadena de conexión

Edita `appsettings.json` y reemplaza `YOUR_DB_PASSWORD` con la contraseña de tu base de datos Supabase:

```
Host=db.ruyjnzsavimrqyulyohu.supabase.co;Port=5432;Database=postgres;
Username=postgres;Password=TU_PASSWORD;SSL Mode=Require;Trust Server Certificate=true
```

O usando variable de entorno en Lambda:

```
DATABASE_URL=Host=db.ruyjnzsavimrqyulyohu.supabase.co;Port=5432;...
```

### 2. Ejecutar localmente

```bash
cd StoreApp.Api
dotnet run
# Swagger disponible en: http://localhost:5000/swagger
```

---

## Despliegue en AWS Lambda

### Opción A — desde la terminal (recomendado)

```bash
cd StoreApp.Api

# Configurar credenciales AWS (si no lo has hecho)
aws configure

# Publicar como Lambda
dotnet lambda deploy-function storeapp-api \
  --region sa-east-1 \
  --function-runtime dotnet10 \
  --function-memory-size 512 \
  --function-timeout 30 \
  --environment-variables "DATABASE_URL=Host=db.ruyjnzsavimrqyulyohu.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=TU_PASSWORD;SSL Mode=Require;Trust Server Certificate=true"
```

### Opción B — usando el archivo de defaults

```bash
dotnet lambda deploy-function
# Usa automáticamente aws-lambda-tools-defaults.json
```

### Conectar API Gateway

Después de desplegar la función, crea un **REST API en API Gateway**:

1. AWS Console → API Gateway → Create API → REST API
2. Integration type: Lambda Function → selecciona `storeapp-api`
3. Deploy API → Stage: `prod`
4. Copia la URL generada (ej. `https://abc123.execute-api.sa-east-1.amazonaws.com/prod`)

---

## Endpoints disponibles

### Brands `/api/brands`
| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/brands` | Listar marcas (paginado, filtro `onlyActive`) |
| GET | `/api/brands/{id}` | Obtener marca por ID |
| POST | `/api/brands` | Crear marca |
| PUT | `/api/brands/{id}` | Actualizar marca |
| DELETE | `/api/brands/{id}` | Desactivar marca (soft delete) |

### Families `/api/families`
| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/families` | Listar familias |
| GET | `/api/families/{id}` | Obtener familia |
| POST | `/api/families` | Crear familia |
| PUT | `/api/families/{id}` | Actualizar familia |
| DELETE | `/api/families/{id}` | Desactivar familia |

### Categories `/api/categories`
| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/categories` | Listar categorías |
| GET | `/api/categories/{id}` | Obtener categoría |
| POST | `/api/categories` | Crear categoría |
| PUT | `/api/categories/{id}` | Actualizar categoría |
| DELETE | `/api/categories/{id}` | Desactivar categoría |

### Products `/api/products`
| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/products` | Listar productos (filtros: `brandId`, `familyId`, `categoryId`, `search`) |
| GET | `/api/products/{id}` | Obtener producto con brand/family/category |
| POST | `/api/products` | Crear producto |
| PUT | `/api/products/{id}` | Actualizar producto |
| DELETE | `/api/products/{id}` | Desactivar producto |

### Images `/api/images`
| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/images?entityId={id}` | Imágenes de una entidad |
| GET | `/api/images/{id}` | Obtener imagen |
| POST | `/api/images` | Crear imagen |
| PUT | `/api/images/{id}` | Actualizar imagen |
| DELETE | `/api/images/{id}` | Desactivar imagen |

### Profiles `/api/profiles`
| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/profiles/{id}` | Obtener perfil de usuario |
| PUT | `/api/profiles/{id}` | Actualizar datos de perfil |

### Cart `/api/users/{userId}/cart`
| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/users/{userId}/cart` | Ver carrito del usuario |
| POST | `/api/users/{userId}/cart` | Agregar/actualizar item en carrito |
| DELETE | `/api/users/{userId}/cart/{productId}` | Eliminar item del carrito |
| DELETE | `/api/users/{userId}/cart` | Vaciar carrito |

### Orders `/api/orders`
| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/orders` | Listar órdenes (filtros: `userId`, `status`) |
| GET | `/api/orders/{id}` | Obtener orden con items |
| POST | `/api/orders?userId={id}` | Crear orden |
| PATCH | `/api/orders/{id}/status` | Actualizar estado de orden |
| DELETE | `/api/orders/{id}` | Cancelar orden |

### Wishlist `/api/users/{userId}/wishlist`
| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/users/{userId}/wishlist` | Ver lista de deseos |
| POST | `/api/users/{userId}/wishlist/{productId}` | Agregar a favoritos |
| DELETE | `/api/users/{userId}/wishlist/{productId}` | Quitar de favoritos |

---

## Parámetros de paginación

Todos los endpoints `GET` de listado aceptan:

| Parámetro | Tipo | Default | Descripción |
|---|---|---|---|
| `page` | int | 1 | Número de página |
| `pageSize` | int | 20 | Registros por página |
| `onlyActive` | bool | true | Filtrar solo registros activos |

Respuesta paginada:
```json
{
  "data": [...],
  "total": 42,
  "page": 1,
  "pageSize": 20
}
```

---

## Estructura del proyecto

```
StoreApp.Api/
├── Controllers/
│   └── Controllers.cs         # Todos los controllers (Brands, Products, Orders, etc.)
├── Data/
│   └── AppDbContext.cs        # EF Core DbContext + configuración de entidades
├── DTOs/
│   ├── Requests/Requests.cs   # DTOs de entrada con validaciones
│   └── Responses/Responses.cs # DTOs de salida
├── Mappings/
│   └── MappingProfile.cs      # Configuración de AutoMapper
├── Middleware/
│   └── ExceptionMiddleware.cs # Manejo global de errores
├── Models/
│   └── Entities.cs            # Entidades del dominio
├── Program.cs                 # Configuración de la aplicación + Lambda hosting
├── appsettings.json
├── aws-lambda-tools-defaults.json
└── StoreApp.Api.csproj
```

---

## Notas importantes

- **Soft delete**: los endpoints `DELETE` desactivan registros (`is_active = false`), no los eliminan físicamente.
- **RLS**: las tablas tienen Row Level Security activo en Supabase. La conexión directa vía EF Core bypasea RLS — asegúrate de manejar la autorización en tu capa de API.
- **Cold start**: el primer request a Lambda puede tardar ~2-3s. Considera habilitar **Provisioned Concurrency** en producción.
- **Connection pooling**: para reducir latencia, considera usar **PgBouncer** o la opción de connection pooling de Supabase en el connection string.
