# NexumApp - Banca Premium

Aplicación bancaria de escritorio desarrollada en C# con Windows Forms que simula las operaciones de una entidad financiera moderna. El proyecto implementa un sistema completo de autenticación, dashboards diferenciados por rol y una interfaz premium con diseño negro/dorado.

**Stack:** Windows Forms, C#, .NET Framework 4.7.2, MySQL (XAMPP), BCrypt

---

## Descripción del Proyecto

NexumApp es una aplicación de banca de escritorio que permite a los usuarios gestionar sus cuentas bancarias, realizar movimientos y transferencias. El sistema diferencia entre dos tipos de usuarios:

- **Administradores:** Acceden a un panel de control con estadísticas del sistema, gestión de usuarios y reportes.
- **Usuarios normales:** Acceden a su dashboard personal donde pueden ver sus cuentas, movimientos recientes y realizar operaciones.

La aplicación está diseñada con un enfoque en la seguridad (contraseñas hasheadas con BCrypt, bloqueo temporal por intentos fallidos) y la experiencia de usuario (notificaciones modernas, validaciones en tiempo real, diseño responsive).

---

## Características Implementadas

### 1. Sistema de Autenticación Completo

#### Login (FrmLogin)
El formulario de inicio de sesión implementa las siguientes características:

- **Validación contra MySQL:** Las credenciales se verifican directamente contra la base de datos.
- **Mensajes de error específicos:** El sistema diferencia entre:
  - Usuario no existente
  - Contraseña incorrecta (con contador de intentos restantes)
  - Usuario desactivado
  - Error de conexión a la base de datos
- **Bloqueo temporal:** Tras 3 intentos fallidos, el usuario queda bloqueado durante 30 segundos.
- **Notificaciones Toast:** Los mensajes de error y éxito aparecen como banners animados en la parte superior, sin ventanas emergentes (MessageBox).
- **Errores inline:** Cada campo muestra su error específico debajo, con el fondo del campo en rojo.
- **Redirección por rol:** Al hacer login exitoso, los administradores van a `FrmDashboardAdmin` y los usuarios normales a `FrmDashboardUsuario`.

#### Registro (FrmRegistro)
El formulario de registro permite crear nuevas cuentas de usuario:

- **Campos requeridos:** Nombre, Apellidos, Email, Contraseña, Confirmar contraseña.
- **Validaciones en tiempo real:**
  - Indicador de fortaleza de contraseña (débil/media/segura) que cambia mientras el usuario escribe.
  - Verificación de coincidencia de contraseñas con feedback visual inmediato.
  - Validación de formato de email.
- **Verificación de email único:** Antes de registrar, se comprueba que el email no exista en la base de datos.
- **Almacenamiento seguro:** Las contraseñas se hashean con BCrypt antes de guardarse.
- **Diseño modal:** El registro se abre sobre el login, permitiendo volver fácilmente.

### 2. Dashboards por Rol

#### Dashboard Administrador (FrmDashboardAdmin)
Panel de control diseñado para administradores del sistema:

- **Header de navegación:** Logo de Nexum Bank + menú (Dashboard, Usuarios, Transferencias, Pagos, Reportes, Cerrar Sesión).
- **Bienvenida personalizada:** Muestra "¡Bienvenido, [Nombre]!" con el nombre del administrador.
- **Panel Resumen del Sistema:**
  - Usuarios Activos: Muestra el total de usuarios registrados.
  - Saldo Total: Suma de todos los saldos del banco.
  - Transacciones del día.
- **Panel Tareas Pendientes:** 4 tarjetas con tareas simuladas (revisar solicitudes, aprobar transferencias, etc.).
- **Panel Gráfico de Actividad:** Área reservada para mostrar estadísticas (actualmente placeholder).
- **Panel Accesos Rápidos:** Botones para Gestionar Usuarios, Ver Reportes y Configuración.
- **Footer:** Copyright y links de Privacidad/Términos.

#### Dashboard Usuario (FrmDashboardUsuario)
Panel personal para usuarios del banco:

- **Header de navegación:** Logo + menú adaptado (Inicio, Cuentas, Transferencias, Pagos, Ayuda, Cerrar Sesión).
- **Banner principal:**
  - Mensaje personalizado "Hola, [Nombre]" + subtítulo motivacional.
  - Botón dorado "ABRIR CUENTA".
  - Fondo oscuro premium.
- **Panel Mis Cuentas:** 3 tarjetas mostrando:
  - Cuenta Corriente con saldo positivo (icono 📈 verde).
  - Cuenta de Ahorros con saldo (icono 🌳 verde).
  - Tarjeta de Crédito con saldo negativo (icono 💳 rojo).
- **Panel Movimientos Recientes:** Lista de 4 movimientos con:
  - Fecha del movimiento.
  - Concepto (Compra en Amazon, Nómina, Spotify, Transferencia).
  - Monto con color (verde para ingresos, rojo para gastos).
  - Botón "VER TODOS" para acceder al historial completo.
- **Panel Accesos Rápidos:** 4 botones para operaciones frecuentes:
  - Hacer Transferencia
  - Pagar Facturas
  - Consultar Movimientos
  - Contactar Soporte
- **Footer:** Links de Sobre Nosotros, Seguridad, Términos y Privacidad.

### 3. Diseño de Interfaz Premium

#### Paleta de Colores
La aplicación utiliza una paleta sofisticada negro/dorado:

| Color | Código | Uso |
|-------|--------|-----|
| Negro Header | #12161E | Barras de navegación y footer |
| Negro Premium | #161616 | Paneles principales |
| Negro Formulario | #121216 | Fondo de formularios |
| Dorado | #D4AF37 | Acentos, botones, texto destacado |
| Blanco | #FFFFFF | Tarjetas y texto principal |
| Gris Fondo | #F0F2F5 | Fondo del área de contenido |
| Verde Éxito | #28A745 | Saldos positivos, notificaciones OK |
| Rojo Error | #DC3545 | Saldos negativos, errores |

#### Sistema de Notificaciones Moderno
En lugar de usar MessageBox tradicionales, implementamos:

- **Toast/Banner:** Panel animado que aparece en la parte superior del formulario.
- **Animación suave:** El banner se despliega y contrae con transición.
- **Auto-ocultado:** Desaparece automáticamente tras 4 segundos.
- **Colores por tipo:** Rojo (error), Amarillo (advertencia), Azul (info), Verde (éxito).
- **Errores inline:** Labels rojos debajo de cada campo con mensaje específico.

#### Elementos de UX
- **Hover effects:** Los botones cambian de color al pasar el cursor.
- **Focus states:** Los campos de texto cambian de fondo cuando están activos.
- **Responsive:** Los formularios se centran automáticamente al redimensionar la ventana.
- **Iconos emoji:** Uso de emojis como iconos visuales (📈, 🌳, 💳, 💸, etc.).

### 4. Seguridad

- **BCrypt:** Todas las contraseñas se hashean antes de almacenarse.
- **Bloqueo temporal:** Protección contra ataques de fuerza bruta.
- **Validación de email único:** Previene duplicados en registro.
- **Sesión singleton:** Control centralizado del usuario logueado.
- **Verificación de estado:** Los usuarios desactivados no pueden acceder.

### 5. Base de Datos

La aplicación utiliza MySQL a través de XAMPP con las siguientes tablas:

- **usuarios:** id, nombre, apellidos, email, passwordHash, esAdmin, activo, fechaRegistro.
- **cuentasbancarias:** id, usuarioId, numeroCuenta, tipoCuenta, saldo, fechaApertura, activa.
- **movimientos:** id, cuentaId, tipoMovimiento, monto, fecha, concepto, saldoAnterior, saldoPosterior.
- **transferencias:** id, cuentaOrigenId, cuentaDestino, nombreBeneficiario, monto, fecha, concepto, estado.

---

## Estructura del Proyecto

```
NexumApp/
├── Data/
│   └── NexumDbContext.cs          # Contexto Entity Framework (✓ implementado)
│
├── Forms/
│   ├── Admin/
│   │   ├── FrmDashboardAdmin      # Dashboard administrador (✓ diseño completo)
│   │   ├── FrmGestionUsuarios     # Gestión de usuarios (pendiente funcionalidad)
│   │   └── FrmReportesAdmin       # Reportes (pendiente funcionalidad)
│   │
│   ├── Auth/
│   │   ├── FrmLogin               # Login completo (✓ implementado)
│   │   └── FrmRegistro            # Registro completo (✓ implementado)
│   │
│   ├── Cuentas/
│   │   ├── FrmAbrirCuenta         # Abrir cuenta (pendiente funcionalidad)
│   │   ├── FrmDetalleCuenta       # Detalle cuenta (pendiente funcionalidad)
│   │   └── FrmMisCuentas          # Listado cuentas (pendiente funcionalidad)
│   │
│   ├── Movimientos/
│   │   ├── FrmHistorialMovimientos    # Historial (pendiente funcionalidad)
│   │   ├── FrmIngresarEfectivo        # Ingresos (pendiente funcionalidad)
│   │   └── FrmRetirarEfectivo         # Retiros (pendiente funcionalidad)
│   │
│   ├── Principal/
│   │   ├── FrmDashboardUsuario    # Dashboard usuario (✓ diseño completo)
│   │   ├── FrmDashboard           # Dashboard genérico (pendiente)
│   │   └── FrmPrincipal           # Ventana temporal (✓ implementado)
│   │
│   └── Transferencias/
│       ├── FrmNuevaTransferencia      # Nueva transferencia (pendiente funcionalidad)
│       └── FrmTransferencias          # Historial transferencias (pendiente funcionalidad)
│
├── Helpers/
│   ├── FormatoMoneda.cs           # Formateo €1,234.56 (pendiente)
│   ├── GeneradorNumeroCuenta.cs   # Generar IBAN (pendiente)
│   ├── PasswordHelper.cs          # Utilidades BCrypt (pendiente)
│   └── Validaciones.cs            # Validar DNI, email, etc. (pendiente)
│
├── Models/
│   ├── CuentaBancaria.cs          # Modelo cuenta (✓ implementado)
│   ├── Movimiento.cs              # Modelo movimiento (✓ implementado)
│   ├── SesionActual.cs            # Singleton sesión (✓ implementado)
│   ├── Transferencia.cs           # Modelo transferencia (✓ implementado)
│   └── Usuario.cs                 # Modelo usuario (✓ implementado)
│
├── Services/
│   ├── AuthService.cs             # Login, registro, validación (✓ implementado)
│   ├── CuentaService.cs           # CRUD cuentas (pendiente)
│   ├── MovimientoService.cs       # Ingresos/retiros (pendiente)
│   └── TransferenciaService.cs    # Transferencias (pendiente)
│
├── Resources/
│   ├── logo.png                   # Logo dorado Nexum (✓)
│   └── background.png             # Fondo login (✓)
│
├── docs/
│   └── MEMORIA_ANTEPROYECTO.md    # Documentación académica (✓)
│
├── Program.cs                     # Punto de entrada (✓)
├── App.config                     # Conexión MySQL (✓)
├── packages.config                # NuGet packages (✓)
└── nexumdb.sql                    # Script BD (✓)
```

---

## Flujo de la Aplicación

```
┌─────────────────────────────────────────────────────────────────┐
│                         INICIO                                  │
└─────────────────────────┬───────────────────────────────────────┘
                          │
                          ▼
┌─────────────────────────────────────────────────────────────────┐
│                       FrmLogin                                  │
│  • Usuario introduce email y contraseña                         │
│  • Sistema valida contra MySQL                                  │
│  • Muestra errores inline o toast según el caso                 │
│  • Bloquea tras 3 intentos fallidos                             │
└─────────────────────────┬───────────────────────────────────────┘
                          │
            ┌─────────────┴─────────────┐
            │                           │
            ▼                           ▼
┌───────────────────────┐   ┌───────────────────────┐
│  Usuario es ADMIN     │   │  Usuario es NORMAL    │
└───────────┬───────────┘   └───────────┬───────────┘
            │                           │
            ▼                           ▼
┌───────────────────────┐   ┌───────────────────────┐
│  FrmDashboardAdmin    │   │  FrmDashboardUsuario  │
│  • Resumen sistema    │   │  • Mis cuentas        │
│  • Tareas pendientes  │   │  • Movimientos        │
│  • Gráfico actividad  │   │  • Accesos rápidos    │
│  • Accesos rápidos    │   │  • Banner personal    │
└───────────────────────┘   └───────────────────────┘
```

---

## Resumen de Implementación

| Componente | Estado | Descripción |
|------------|--------|-------------|
| FrmLogin | ✅ Completo | Login con validaciones, toast, bloqueo temporal |
| FrmRegistro | ✅ Completo | Registro con indicador fortaleza, validaciones |
| FrmDashboardAdmin | ✅ Diseño | Panel admin con todos los elementos visuales |
| FrmDashboardUsuario | ✅ Diseño | Panel usuario con cuentas y movimientos |
| AuthService | ✅ Completo | Login, registro, verificación email |
| Models | ✅ Completo | Usuario, CuentaBancaria, Movimiento, Transferencia, SesionActual |
| NexumDbContext | ✅ Completo | Contexto EF con relaciones |
| Base de datos | ✅ Completo | Script SQL con 4 tablas |

---

## Instalación y Ejecución

### Requisitos
- Visual Studio 2019/2022 con .NET Framework 4.7.2
- XAMPP con MySQL
- 8GB RAM mínimo recomendado

### Pasos

1. **Iniciar XAMPP:**
   - Abrir XAMPP Control Panel
   - Iniciar Apache y MySQL (deben estar en verde)

2. **Crear base de datos:**
   - Ir a http://localhost/phpmyadmin
   - Crear nueva base de datos: `nexumdb`
   - Seleccionar cotejamiento: `utf8_general_ci`
   - Importar archivo `nexumdb.sql`

3. **Ejecutar aplicación:**
   - Abrir `NexumApp.sln` en Visual Studio
   - Compilar: Ctrl + Shift + B
   - Ejecutar: F5

### Credenciales de prueba

| Email | Contraseña | Tipo | Destino |
|-------|------------|------|---------|
| admin@nexum.com | 123456 | Administrador | FrmDashboardAdmin |
| juan@email.com | 123456 | Usuario | FrmDashboardUsuario |
| test@test.com | 123456 | Usuario | FrmDashboardUsuario |

---

## Tecnologías Utilizadas

| Tecnología | Versión | Propósito |
|------------|---------|-----------|
| C# | - | Lenguaje principal |
| .NET Framework | 4.7.2 | Runtime |
| Windows Forms | - | Interfaz gráfica |
| MySQL | 8.x | Base de datos |
| XAMPP | 8.x | Servidor local MySQL |
| MySql.Data | 8.0.33 | Conector ADO.NET |
| Entity Framework | 6.4.4 | ORM |
| BCrypt.Net-Next | 4.1.0 | Hash de contraseñas |

---

## Funcionalidades Pendientes

Para completar la aplicación, quedan por implementar:

1. **Gestión de Cuentas:** Crear, ver y cerrar cuentas bancarias.
2. **Movimientos:** Realizar ingresos y retiros de efectivo.
3. **Transferencias:** Enviar dinero entre cuentas con validaciones.
4. **Historial:** Ver todos los movimientos con filtros y búsqueda.
5. **Panel Admin:** Funcionalidad real para gestionar usuarios y ver reportes.
6. **Gráficos:** Implementar gráficos de actividad con datos reales.

---

## Documentación Adicional

- **Memoria del Anteproyecto:** `docs/MEMORIA_ANTEPROYECTO.md`
  - Portada, título, descripción
  - Objetivos generales y específicos
  - Módulos profesionales implicados
  - Metodología y cronograma
  - Prueba de concepto (PoC)

---

## Autores

Proyecto desarrollado para el Ciclo Formativo de Grado Superior en Desarrollo de Aplicaciones Multiplataforma (DAM).

---

## Licencia

Uso exclusivamente educativo.
