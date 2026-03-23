# Reporte Final: Migración Proyecto Todo Para Tu TractoCamión

Este documento resume todo el proceso de migración de la infraestructura de local/Railway a la combinación de **Supabase + Render + GitHub Pages**, incluyendo el historial de desafíos técnicos y sus soluciones definitivas.

---

## 🚀 Resumen del Proyecto

El objetivo principal fue migrar una aplicación .NET Core (API + Blazor WASM) a un entorno gratuito y estable, resolviendo problemas de conexión a base de datos y despliegue en la nube.

- **Base de Datos:** PostgreSQL en Supabase.
- **Backend:** .NET 10 API en Render.
- **Frontend:** Blazor WebAssembly en GitHub Pages.

---

## 🛠️ Desafíos Técnicos y Soluciones

### 1. Conexión IPv4 vs IPv6 (Supabase)
**Problema:** Render no soporta IPv6 de forma nativa para conexiones externas, lo que causaba un error 500 al inicio.
**Solución:** Se utilizó el **Pooler de Supabase** en el puerto `6543`, que permite conexiones IPv4 estables.

### 2. Mapeo de Tablas y Case Sensitivity (PostgreSQL)
**Problema:** PostgreSQL es sensible a las mayúsculas/minúsculas. La tabla se llamaba "Producto" (singular) y las columnas tenían nombres mezclados (`id`, `nombre`, `imagen1Url`).
**Solución:** Se configuró el `DbContext` para mapear explícitamente cada campo:
- Tabla: `ToTable("Producto")`
- IDs: Conversión de `Text` a `Guid`.
- Precios: Conversión de `Double Precision` a `Decimal`.

### 3. Soporte para .NET 10
**Problema:** Al ser una versión muy reciente (.NET 10), las plataformas de despliegue fallaban por falta del SDK correcto.
**Solución:** 
- En **Render**, se ajustó el `Dockerfile` para usar la imagen oficial de .NET 10.
- En **GitHub Actions**, se configuró el paso `setup-dotnet` con la calidad `preview` y versión `10.0.x`.

### 4. Permisos de Despliegue en GitHub
**Problema:** El flujo de publicación automático fallaba con error 128 (bloqueo de escritura).
**Solución:** Se añadió el bloque `permissions: contents: write` al flujo de trabajo para permitir que GitHub Pages guarde los archivos compilados.

---

## 📂 Inventario de Evidencia Visual

A continuación, se presentan las capturas de pantalla de los errores resueltos durante el proceso (puedes verlas en tu carpeta de archivos):

![Captura Error 500](file:///C:/Users/German/.gemini/antigravity/brain/1045f6da-1d74-4c21-b72b-3a5da3cb8ca4/media__1774170703363.png)
*Error inicial de conexión a base de datos.*

![Validación Swagger](file:///C:/Users/German/.gemini/antigravity/brain/1045f6da-1d74-4c21-b72b-3a5da3cb8ca4/media__1774180417183.png)
*Pruebas exitosas de carga de datos en Swagger.*

![Despliegue GitHub Actions](file:///C:/Users/German/.gemini/antigravity/brain/1045f6da-1d74-4c21-b72b-3a5da3cb8ca4/media__1774181297867.png)
*Historial de compilación y publicación automática.*

---

## 🔗 Enlaces Útiles del Proyecto

- **Página Web:** [https://german3.github.io/TodoParaTuTractoCamion/](https://german3.github.io/TodoParaTuTractoCamion/)
- **Documentación API (Swagger):** [https://todoparatutractocamion.onrender.com/swagger/index.html](https://todoparatutractocamion.onrender.com/swagger/index.html)
- **Base de Datos:** [Supabase Dashboard](https://supabase.com/dashboard/project/vljps5)

---

**Nota Técnica:** Este documento ha sido generado en formato Markdown para preservar la alta resolución de las imágenes y el resaltado de código. Puedes abrirlo con cualquier editor de texto o Word para exportarlo a PDF si lo deseas.

¡Gracias por la confianza en este proceso de migración!
