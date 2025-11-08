# 🔧 Guía Paso a Paso: Configurar GH_PAT para GitHub Pages

## ❓ ¿Por qué necesito esto?

El workflow `publish_cov_report.yml` falla con este error:
```
fatal: Authentication failed
Error: The deploy step encountered an error: The process '/usr/bin/git' failed with exit code 128
```

Esto ocurre porque en repositorios de **organizaciones** (como UPT-FAING-EPIS), el token `GITHUB_TOKEN` predeterminado no tiene permisos para hacer push a la rama `gh-pages`.

## 🎯 Solución: Crear y configurar un Personal Access Token

### Paso 1: Crear el Token (5 minutos)

1. **Inicia sesión en GitHub** con tu cuenta (diegocastillo12)

2. **Abre la configuración de tokens:**
   - Click en tu foto de perfil (arriba a la derecha)
   - Settings
   - Developer settings (al final del menú izquierdo)
   - Personal access tokens → Tokens (classic)
   - O directamente: https://github.com/settings/tokens

3. **Genera un nuevo token:**
   - Click en "Generate new token" → "Generate new token (classic)"

4. **Configura el token:**
   ```
   Note (nombre): GH_PAT_LAB06_SI784
   Expiration: 90 days (o Custom para más tiempo)
   
   Selecciona estos scopes (permisos):
   ☑️ repo
      ☑️ repo:status
      ☑️ repo_deployment
      ☑️ public_repo
      ☑️ repo:invite
      ☑️ security_events
   ☑️ workflow
   ```

5. **Genera y copia el token:**
   - Click en "Generate token" (botón verde al final)
   - **⚠️ IMPORTANTE:** Copia el token inmediatamente (empieza con `ghp_...`)
   - Solo se muestra UNA VEZ. Si lo pierdes, tendrás que crear uno nuevo

### Paso 2: Agregar el Token al Repositorio (3 minutos)

1. **Ve a tu repositorio:**
   - https://github.com/UPT-FAING-EPIS/lab-2025-ii-si784-u2-06-cs-diegocastillo12

2. **Abre la configuración de Secrets:**
   - Click en "Settings" (en el menú del repo)
   - En el menú izquierdo: "Secrets and variables" → "Actions"

3. **Crea el secreto:**
   - Click en "New repository secret"
   - Name: `GH_PAT` (exactamente así, respetando mayúsculas)
   - Secret: Pega el token que copiaste (ghp_...)
   - Click en "Add secret"

### Paso 3: Configurar GitHub Pages (2 minutos)

1. **Activa GitHub Pages:**
   - En tu repo, ve a Settings → Pages
   - Source: Selecciona "Deploy from a branch"
   - Branch: Selecciona `gh-pages` / `/ (root)`
   - Click en "Save"

### Paso 4: Re-ejecutar el Workflow (1 minuto)

1. **Ve a la pestaña Actions:**
   - En tu repositorio, click en "Actions"

2. **Encuentra el workflow fallido:**
   - Click en "CI - Tests, Coverage and Publish to Pages"
   - Selecciona el run más reciente (el que falló)

3. **Re-ejecuta:**
   - Click en "Re-run failed jobs" o "Re-run all jobs"
   - Espera 2-3 minutos

4. **Verifica el éxito:**
   - Todos los jobs deben aparecer en verde ✅
   - El reporte estará disponible en: https://upt-faing-epis.github.io/lab-2025-ii-si784-u2-06-cs-diegocastillo12/

## ✅ Verificación Final

Después de completar los pasos:

1. **El workflow debe completarse exitosamente:**
   - ✅ build-and-test
   - ✅ publish-pages

2. **GitHub Pages debe estar activo:**
   - Ve a Settings → Pages
   - Debe decir: "Your site is live at https://upt-faing-epis.github.io/..."

3. **El reporte de cobertura debe ser accesible:**
   - Abre: https://upt-faing-epis.github.io/lab-2025-ii-si784-u2-06-cs-diegocastillo12/
   - Debes ver el reporte HTML de cobertura con gráficos

## 🆘 Problemas Comunes

### "No veo la opción Developer settings"
- Asegúrate de estar en tu perfil de usuario, no en el de la organización
- La ruta es: Tu foto → Settings → Developer settings

### "El workflow sigue fallando después de agregar GH_PAT"
- Verifica que el nombre del secreto sea exactamente `GH_PAT` (mayúsculas)
- Asegúrate de haber seleccionado los scopes `repo` y `workflow`
- Intenta re-generar el token y agregarlo nuevamente

### "No tengo permisos para agregar secretos"
- Necesitas ser colaborador del repositorio con permisos de admin
- Contacta al profesor o al administrador del repositorio

### "El token expiró"
- Genera un nuevo token siguiendo el Paso 1
- Actualiza el secreto existente en el Paso 2 (Edit secret)

## 📝 Notas Importantes

- **Seguridad:** Nunca compartas tu token en código, issues, o commits
- **Expiración:** Los tokens expiran. Si el workflow falla en el futuro, verifica la expiración
- **Backup:** Guarda el token en un lugar seguro (como un password manager)
- **Revocar:** Si el token se compromete, revócalo inmediatamente en: https://github.com/settings/tokens

## 🎓 ¿Por qué es necesario en repos de organizaciones?

En repos personales, `GITHUB_TOKEN` tiene permisos completos por defecto. Pero en repos de organizaciones, GitHub restringe estos permisos por seguridad. Por eso necesitamos un token personal con permisos explícitos.

## 🔗 Referencias

- [GitHub Personal Access Tokens](https://docs.github.com/en/authentication/keeping-your-account-and-data-secure/creating-a-personal-access-token)
- [GitHub Actions Secrets](https://docs.github.com/en/actions/security-guides/encrypted-secrets)
- [GitHub Pages Deploy Action](https://github.com/JamesIves/github-pages-deploy-action)

---

**¿Todo listo?** Una vez configurado, todos tus workflows funcionarán automáticamente en cada push. 🚀
