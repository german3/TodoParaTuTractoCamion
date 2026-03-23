-- ==========================================================
-- SCRIPT DE ACTUALIZACIÓN DE IMÁGENES (SUPABASE)
-- Ejecutar en el SQL Editor de Supabase
-- ==========================================================

-- 1. ZEPELIN COMPLETO KW 22 LED AMBAR
UPDATE "Producto" 
SET "imagen1Url" = 'images/productos/img_final_r146_428.png' 
WHERE "nombre" ILIKE '%ZEPELIN COMPLETO KW 22%';

-- 2. LIMPIAPARABRISAS HELLA CLEANTECH 21"
UPDATE "Producto" 
SET "imagen1Url" = 'images/productos/img_final_r277_793.png' 
WHERE "nombre" ILIKE '%HELLA CLEANTECH 21%';

-- 3. FARO INTERNATIONAL 4300 L
UPDATE "Producto" 
SET "imagen1Url" = 'images/productos/img_final_r91_270.png' 
WHERE "nombre" ILIKE '%FARO INTERNATIONAL 4300%';

-- 4. ALAMBRON PORTA LODERA SENCILLOS M
UPDATE "Producto" 
SET "imagen1Url" = 'images/productos/img_final_r284_811.png' 
WHERE "nombre" ILIKE '%ALAMBRON PORTA LODERA%';

-- 5. TTFCU00004 / IPS TTFCU00004 (JGO)
UPDATE "Producto" 
SET "imagen1Url" = 'images/productos/img_final_r385_980.jpeg' 
WHERE "nombre" ILIKE '%TTFCU00004%';

-- 6. TAPETE INTERNATIONAL PROSTAR
UPDATE "Producto" 
SET "imagen1Url" = 'images/productos/img_final_r44_135.png' 
WHERE "nombre" ILIKE '%TAPETE INTERNATIONAL%PROSTAR%';
