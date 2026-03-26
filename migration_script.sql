CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260326014744_Migracion_893_Productos') THEN
    CREATE TABLE "Producto" (
        id text NOT NULL,
        nombre text NOT NULL,
        precio double precision NOT NULL,
        stock integer NOT NULL,
        "imagen1Url" text,
        "imagen2Url" text,
        "imagen3Url" text,
        detalles text,
        categoria text,
        CONSTRAINT "PK_Producto" PRIMARY KEY (id)
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260326014744_Migracion_893_Productos') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260326014744_Migracion_893_Productos', '10.0.4');
    END IF;
END $EF$;
COMMIT;

