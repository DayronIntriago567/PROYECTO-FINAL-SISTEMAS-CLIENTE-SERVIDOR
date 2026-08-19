# Especificación de servicios Web – BikeStore API

Base URL: `https://localhost:5001/api`
Formato: JSON | Autenticación: no requerida en esta versión | Códigos HTTP estándar (200, 201, 400, 404, 500)

---

## 1. Categorías – `/api/categorias`

| Método | Endpoint | Descripción |
|---|---|---|
| GET | /api/categorias | Lista todas las categorías |
| GET | /api/categorias/{id} | Obtiene una categoría por id |
| POST | /api/categorias | Crea una categoría |
| PUT | /api/categorias/{id} | Actualiza una categoría |
| DELETE | /api/categorias/{id} | Elimina (o desactiva) una categoría |

**POST /api/categorias – Request**
```json
{
  "nombre": "Montaña",
  "descripcion": "Bicicletas para terreno irregular",
  "activo": true
}
```

**Response 201**
```json
{
  "idCategoria": 1,
  "nombre": "Montaña",
  "descripcion": "Bicicletas para terreno irregular",
  "activo": true
}
```

---

## 2. Bicicletas – `/api/bicicletas`

| Método | Endpoint | Descripción |
|---|---|---|
| GET | /api/bicicletas | Lista todas las bicicletas |
| GET | /api/bicicletas/{id} | Obtiene una bicicleta por id |
| GET | /api/bicicletas/buscar?categoria=&marca= | Busca por categoría y/o marca |
| GET | /api/bicicletas/stock-bajo | Bicicletas con stock bajo o agotado |
| POST | /api/bicicletas | Registra una bicicleta |
| PUT | /api/bicicletas/{id} | Actualiza precio y/o stock |
| DELETE | /api/bicicletas/{id} | Elimina una bicicleta |

**POST /api/bicicletas – Request**
```json
{
  "idCategoria": 1,
  "marca": "Trek",
  "modelo": "Marlin 7",
  "precio": 950.00,
  "stock": 10,
  "estado": "Activo"
}
```

**Response 201**
```json
{
  "idBicicleta": 15,
  "idCategoria": 1,
  "marca": "Trek",
  "modelo": "Marlin 7",
  "precio": 950.00,
  "stock": 10,
  "estado": "Activo"
}
```

**PUT /api/bicicletas/{id} – Request**
```json
{
  "precio": 899.00,
  "stock": 8
}
```
**Response**: `200 OK` con el objeto actualizado, o `404 Not Found` si el id no existe.

**DELETE /api/bicicletas/{id} – Response**: `204 No Content`, o `404 Not Found`.

---

## 3. Clientes – `/api/clientes`

| Método | Endpoint | Descripción |
|---|---|---|
| GET | /api/clientes | Lista todos los clientes |
| GET | /api/clientes/{id} | Obtiene un cliente por id |
| GET | /api/clientes/buscar?cedula=&apellido= | Busca por cédula o apellido |
| POST | /api/clientes | Registra un cliente |
| PUT | /api/clientes/{id} | Actualiza un cliente |
| DELETE | /api/clientes/{id} | Elimina un cliente |

**POST /api/clientes – Request**
```json
{
  "cedula": "1712345678",
  "nombres": "Ana",
  "apellidos": "Pérez",
  "telefono": "0991234567",
  "correo": "ana.perez@mail.com"
}
```

---

## 4. Ventas – `/api/ventas`

| Método | Endpoint | Descripción |
|---|---|---|
| GET | /api/ventas | Historial de ventas |
| GET | /api/ventas/{id} | Detalle de una venta |
| GET | /api/ventas/cliente/{idCliente} | Ventas realizadas por un cliente |
| POST | /api/ventas | Registra una venta (con detalle) |

**POST /api/ventas – Request**
```json
{
  "idCliente": 3,
  "detalle": [
    { "idBicicleta": 15, "cantidad": 2 },
    { "idBicicleta": 7, "cantidad": 1 }
  ]
}
```
El servidor calcula `precio` y `subtotal` por línea a partir del precio vigente de cada bicicleta, calcula IVA y `total` de la venta, y descuenta el stock correspondiente.

**Response 201**
```json
{
  "idVenta": 22,
  "fecha": "2026-08-17T10:30:00",
  "idCliente": 3,
  "subtotal": 2799.00,
  "iva": 335.88,
  "total": 3134.88,
  "detalle": [
    { "idBicicleta": 15, "cantidad": 2, "precio": 899.00, "subtotal": 1798.00 },
    { "idBicicleta": 7, "cantidad": 1, "precio": 1001.00, "subtotal": 1001.00 }
  ]
}
```
**Errores**: `400 Bad Request` si el stock de alguna bicicleta es insuficiente; `404 Not Found` si el cliente o una bicicleta no existen.

---

## Manejo de errores (formato común)

```json
{
  "error": "Stock insuficiente para la bicicleta 15",
  "codigo": 400
}
```
