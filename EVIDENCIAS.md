# Evidencias - Lab 06

Estudiante: Diego Fernando Castillo Mamani

Entregables añadidos al repositorio:

- `UPTSiteTests/UPTSiteTests.csproj` — proyecto de pruebas (MSTest + Playwright)
- `UPTSiteTests/UPTSiteTest.cs` — clase con pruebas (3 del enunciado + 2 nuevas)
- `.github/workflows/publish_cov_report.yml` — CI para ejecutar pruebas, generar reporte de cobertura y publicar en Pages
- `.github/workflows/release.yml` — CI para empaquetar como NuGet (versión = matrícula vía input) y publicar en GitHub Packages + crear Release
- `EVIDENCIAS.md` — este archivo

Cómo ejecutar las pruebas localmente (Windows / PowerShell):

1. Crear la solución y proyecto (si deseas recrear desde 0):

```powershell
# desde la carpeta del repo
cd "c:\Users\Mi Equipo\3D Objects\LABORATORIOS CALIDAD\LAB_06\lab-2025-ii-si784-u2-06-cs-diegocastillo12"
dotnet restore UPTSiteTests/UPTSiteTests.csproj
dotnet build UPTSiteTests/UPTSiteTests.csproj
```

2. Instalar navegadores Playwright (después del build el script pwsh se encuentra en bin):

```powershell
# Ejecutar desde PowerShell (o pwsh)
pwsh bin/Debug/net8.0/playwright.ps1 install
```

3. Ejecutar las pruebas y recolectar cobertura:

```powershell
dotnet test UPTSiteTests/UPTSiteTests.csproj --collect:"XPlat Code Coverage"
```

4. Generar el reporte HTML (ReportGenerator):

```powershell
dotnet tool install -g dotnet-reportgenerator-globaltool
ReportGenerator "-reports:./*/*/*/coverage.cobertura.xml" "-targetdir:Cobertura" -reporttypes:HTML
# luego abrir Cobertura/index.htm en un navegador
```

Cómo ver trazas y videos:

- Las trazas generadas por Playwright se guardan en `playwright-traces/` con nombre {TestClass}.{TestName}.zip — se pueden abrir con el Playwright Trace Viewer: `pwsh bin/Debug/net8.0/playwright.ps1 show-trace <ruta>`.
- Los videos se graban en la carpeta `videos/` dentro del directorio donde se ejecute el test.

Notas sobre CI y release:

- El flujo `publish_cov_report.yml` ejecuta las pruebas en CI, genera el reporte HTML con ReportGenerator y despliega la carpeta `Cobertura` a GitHub Pages mediante `actions/deploy-pages`. También sube los videos y trazas como artefactos.
- El flujo `release.yml` puede ejecutarse manualmente (workflow_dispatch) y requiere el input `matricula` (por ejemplo: `20201234`) que se usará como versión del paquete NuGet. También reacciona a pushes por tag `v*`.

Asunciones realizadas:

- "SOT" no estaba completamente especificado; supuse que te referías a evidencia/documentación (por eso añadí `EVIDENCIAS.md`). Si SOT significa otra cosa, indícalo y lo ajusto.
- No se modificó `classroom.yml` en ningún momento.

Resultados que deberías ver tras ejecutar localmente:

- `dotnet test` : salida indicando Passed/Failed count.
- Carpeta `playwright-traces/` con archivos zip por test.
- Carpeta `videos/` con los mp4 de las ejecuciones.
- Carpeta `Cobertura/index.htm` con reporte HTML (tras ejecutar ReportGenerator).
