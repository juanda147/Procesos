# Backup y Restauracion de Base de Datos - ProcesosDb

## Backup Automatico

La aplicacion genera backups automaticos de MongoDB cada 30 dias (configurable) y los envia por correo electronico como archivo ZIP.

### Configuracion

En `backend/ProcesosApi/appsettings.json`:

```json
"Backup": {
  "IntervaloDias": 30,
  "CorreoDestino": ""
}
```

- **IntervaloDias**: Cada cuantos dias se genera el backup (por defecto 30).
- **CorreoDestino**: Email donde se envia el backup. Si esta vacio, usa `Email.CorreoPorDefecto`.

Tambien debe estar configurada la seccion `Email` con las credenciales SMTP de Gmail.

### Contenido del Backup

El archivo ZIP contiene 3 archivos JSON:

| Archivo              | Coleccion MongoDB | Contenido                        |
|----------------------|-------------------|----------------------------------|
| `procesos.json`      | procesos          | Procesos con pagos, notas, recordatorios |
| `catalogos.json`     | catalogos         | Tipos de proceso, ciudades, juzgados, etc. |
| `campos_globales.json` | camposGlobales  | Campos personalizados globales   |

---

## Restauracion

### Requisitos

- MongoDB corriendo en `localhost:27017` (o la URL configurada)
- [MongoDB Database Tools](https://www.mongodb.com/try/download/database-tools) instalado (para `mongoimport`)

### Opcion 1: Usando mongoimport (Recomendado)

1. Descomprimir el archivo ZIP del backup:
   ```
   ProcesosDb_backup_2026-03-20.zip
   ```

2. Abrir una terminal en la carpeta donde se descomprimieron los archivos.

3. Importar cada coleccion. Usar `--drop` para reemplazar los datos existentes:

   ```bash
   mongoimport --db ProcesosDb --collection procesos --file procesos.json --jsonArray --drop

   mongoimport --db ProcesosDb --collection catalogos --file catalogos.json --jsonArray --drop

   mongoimport --db ProcesosDb --collection camposGlobales --file campos_globales.json --jsonArray --drop
   ```

4. Reiniciar la aplicacion para que se recreen los indices.

### Opcion 2: Usando MongoDB Compass (Interfaz grafica)

1. Descomprimir el ZIP del backup.

2. Abrir [MongoDB Compass](https://www.mongodb.com/products/compass) y conectar a `mongodb://localhost:27017`.

3. Seleccionar la base de datos `ProcesosDb`.

4. Para cada coleccion (`procesos`, `catalogos`, `camposGlobales`):
   - Click en la coleccion
   - Click en "Add Data" > "Import JSON or CSV file"
   - Seleccionar el archivo JSON correspondiente
   - Marcar "Drop collection before import" si se quiere reemplazar los datos
   - Click en "Import"

5. Reiniciar la aplicacion.

### Opcion 3: Usando mongosh (MongoDB Shell)

1. Descomprimir el ZIP del backup.

2. Abrir `mongosh` y conectar:
   ```
   mongosh mongodb://localhost:27017/ProcesosDb
   ```

3. Para cada coleccion, cargar el JSON y reemplazar:
   ```javascript
   // Leer el archivo JSON
   const procesos = JSON.parse(fs.readFileSync('procesos.json', 'utf8'));

   // Limpiar la coleccion e insertar
   db.procesos.drop();
   db.procesos.insertMany(procesos);

   // Repetir para las otras colecciones
   const catalogos = JSON.parse(fs.readFileSync('catalogos.json', 'utf8'));
   db.catalogos.drop();
   db.catalogos.insertMany(catalogos);

   const campos = JSON.parse(fs.readFileSync('campos_globales.json', 'utf8'));
   db.camposGlobales.drop();
   db.camposGlobales.insertMany(campos);
   ```

4. Reiniciar la aplicacion para que se recreen los indices.

---

## Verificacion despues de restaurar

1. Abrir la aplicacion en `http://localhost:5173`
2. Verificar que el dashboard muestra los procesos correctos
3. Abrir algunos procesos y verificar pagos, notas y recordatorios
4. Ir a Configuracion y verificar que los catalogos estan completos

---

## Notas importantes

- **Limite de Gmail**: Los adjuntos no pueden superar 25 MB. Para una aplicacion local de gestion legal, los datos comprimidos normalmente estan muy por debajo de este limite.
- **Datos sensibles**: El backup contiene toda la informacion de los procesos. Mantener los archivos ZIP en un lugar seguro.
- **Primer backup**: Se ejecuta 30 segundos despues de iniciar la aplicacion, y luego cada N dias segun la configuracion.
