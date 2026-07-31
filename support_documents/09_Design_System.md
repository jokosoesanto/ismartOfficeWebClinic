# 09 Design System

## Overview
Design System Clinic Web dibangun menggunakan pendekatan **CSS Custom Properties (Variables)** atau **Design Tokens** yang tertanam dalam file `themes.css`. Pendekatan ini memungkinkan aplikasi memiliki antarmuka yang bersih, dapat dikustomisasi, dan profesional (mengusung nuansa _Corporate Medical_).

## Kerangka Dasar
Aplikasi menggunakan **Bootstrap 5** sebagai _base CSS framework_ dengan ikon pendukung **Bootstrap Icons**. Namun, seluruh warna elemen HTML di-override menggunakan _Design Tokens_ untuk menghindari hardcoding warna statis di View.

## Design Tokens Default (Medical Blue)
- `--primary-color`: `#0d6efd`
- `--secondary-color`: `#6c757d`
- `--bg-main`: `#f8f9fa`
- `--bg-card`: `#ffffff`
- `--bg-sidebar`: `#ffffff`
- `--bg-header`: `#ffffff`
- `--text-main`: `#212529`
- `--border-color`: `#dee2e6`
- `--sidebar-width`: `250px`
- `--header-height`: `60px`

Semua komponen kustom (seperti `clinic-card`) dirancang agar selalu menyesuaikan warnanya secara otomatis dengan nilai _Design Tokens_ tersebut. Hal ini menghapus *technical debt* berupa hardcoded CSS pada view, dan memastikan desain merata (konsisten) di seluruh _module prototype_.
