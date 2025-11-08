# Solución al Error de CI/CD - GitHub Pages Deploy

## 🔴 Error Actual

```
fatal: Authentication failed for 'https://github.com/UPT-FAING-EPIS/lab-2025-ii-si784-u2-06-cs-diegocastillo12.git/'
Error: The deploy step encountered an error: The process '/usr/bin/git' failed with exit code 128 ❌
```

## 🎯 Causa del Problema

El workflow `publish_cov_report.yml` intenta publicar en GitHub Pages usando la rama `gh-pages`, pero en repositorios de **organizaciones** (como UPT-FAING-EPIS), el token `GITHUB_TOKEN` por defecto **NO tiene permisos** para hacer push.

## ✅ Soluciones

### Opción 1: Crear y Configurar GH_PAT (RECOMENDADO)

1. **Crear un Personal Access Token (PAT)**:
   - Ve a: https://github.com/settings/tokens
   - Click en "Generate new token" → "Generate new token (classic)"
   - Nombre: `GH_PAT_LAB06` o similar
   - Expiration: 90 días (o lo que prefieras)
   - Selecciona los siguientes scopes:
     - ✅ `repo` (Full control of private repositories)
     - ✅ `workflow` (Update GitHub Action workflows)
   - Click en "Generate token"
   - **COPIA EL TOKEN** (solo se muestra una vez)

2. **Agregar el secreto al repositorio**:
   - Ve a tu repositorio: https://github.com/UPT-FAING-EPIS/lab-2025-ii-si784-u2-06-cs-diegocastillo12
   - Settings → Secrets and variables → Actions
   - Click en "New repository secret"
   - Name: `GH_PAT`
   - Secret: (pega el token copiado)
   - Click en "Add secret"

3. **Re-ejecutar el workflow**:
   - Ve a la pestaña "Actions"
   - Selecciona el workflow fallido
   - Click en "Re-run failed jobs"

### Opción 2: Usar GitHub Pages con artifacts (Alternativa)

Si no puedes crear un PAT, puedes modificar el workflow para usar el método oficial de GitHub Pages:

**Modificar el job `publish-pages`** en `.github/workflows/publish_cov_report.yml`:

```yaml
  publish-pages:
    needs: build-and-test
    runs-on: ubuntu-latest
    permissions:
      pages: write
      id-token: write
    environment:
      name: github-pages
      url: ${{ steps.deployment.outputs.page_url }}
    steps:
      - name: Download cobertura artifact
        uses: actions/download-artifact@v4
        with:
          name: cobertura-html
          path: cobertura-html

      - name: Setup Pages
        uses: actions/configure-pages@v4

      - name: Upload artifact for Pages
        uses: actions/upload-pages-artifact@v3
        with:
          path: cobertura-html

      - name: Deploy to GitHub Pages
        id: deployment
        uses: actions/deploy-pages@v4
```

**Y habilitar GitHub Pages en el repositorio**:
1. Settings → Pages
2. Source: "GitHub Actions"
3. Guardar

### Opción 3: Cambiar los permisos del GITHUB_TOKEN (Si eres admin)

Si tienes permisos de administrador en la organización:

1. Ve a: https://github.com/organizations/UPT-FAING-EPIS/settings/actions
2. En "Workflow permissions", selecciona "Read and write permissions"
3. Guarda los cambios

Luego, modifica el workflow para usar `github.token` directamente:

```yaml
      - name: Deploy Cobertura to GitHub Pages (gh-pages)
        uses: JamesIves/github-pages-deploy-action@v4
        with:
          branch: gh-pages
          folder: cobertura-html
          token: ${{ github.token }}
```

## 📝 Estado Actual del Workflow

He actualizado el archivo `.github/workflows/publish_cov_report.yml` para intentar usar `GH_PAT` si está disponible, o fallar de manera más clara:

```yaml
token: ${{ secrets.GH_PAT || github.token }}
```

Esto intentará usar `GH_PAT` primero, y si no existe, usará `github.token` (aunque probablemente falle en repos de org).

## 🔍 Verificación

Después de aplicar cualquiera de las soluciones:

1. Ve a la pestaña "Actions"
2. El workflow debe completarse exitosamente
3. Verifica que GitHub Pages esté publicado en:
   - https://upt-faing-epis.github.io/lab-2025-ii-si784-u2-06-cs-diegocastillo12/

## ✨ Resumen de Actividades Completadas

- ✅ **Actividad 1**: 2 escenarios de prueba adicionales agregados
  - `CheckAdmissionsPageContainsAdmissionsText()`
  - `CheckFooterContainsContact()`

- ✅ **Actividad 2**: Workflow `publish_cov_report.yml` creado
  - Compila el proyecto
  - Ejecuta las pruebas con cobertura
  - Genera reporte HTML de cobertura
  - Sube videos y trazas como artifacts
  - Publica el reporte en GitHub Pages

- ✅ **Actividad 3**: Workflow `release.yml` creado
  - Genera NuGet con código de matrícula como versión
  - Publica el paquete en GitHub Packages
  - Crea el Release correspondiente

---

**Recomendación**: Usa la **Opción 1** (crear GH_PAT) para una solución rápida y efectiva.
