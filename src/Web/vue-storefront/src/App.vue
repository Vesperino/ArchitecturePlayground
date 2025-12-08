<script setup lang="ts">
import { RouterLink, RouterView } from 'vue-router'
</script>

<template>
  <div class="min-h-screen bg-gray-50 flex flex-col">
    <header class="bg-white shadow">
      <nav class="max-w-7xl mx-auto px-4 py-4">
        <div class="flex items-center justify-between">
          <RouterLink to="/" class="text-2xl font-bold text-gray-900 hover:text-blue-600">
            ArchitecturePlayground
          </RouterLink>

          <div class="flex items-center gap-6">
            <RouterLink
              to="/products"
              class="text-gray-600 hover:text-blue-600 font-medium"
            >
              Products
            </RouterLink>
            <RouterLink
              to="/cart"
              class="text-gray-600 hover:text-blue-600 font-medium"
            >
              Cart
            </RouterLink>
            <RouterLink
              to="/orders"
              class="text-gray-600 hover:text-blue-600 font-medium"
            >
              Orders
            </RouterLink>
            <RouterLink
              to="/login"
              class="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700"
            >
              Login
            </RouterLink>
          </div>
        </div>
      </nav>
    </header>

    <main class="flex-1">
      <RouterView />
    </main>

    <footer class="bg-white border-t">
      <div class="max-w-7xl mx-auto px-4 py-4 text-center text-gray-500 text-sm">
        &copy; 2025 ArchitecturePlayground - Modular Monolith + Vertical Slice Architecture Demo
      </div>
    </footer>
  </div>
</template>
