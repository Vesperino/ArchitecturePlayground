<script setup lang="ts">
import { RouterView } from 'vue-router'
</script>

<template>
  <div class="min-h-screen bg-gray-50">
    <header class="bg-white shadow">
      <div class="max-w-7xl mx-auto px-4 py-6">
        <h1 class="text-3xl font-bold text-gray-900">
          ArchitecturePlayground Store
        </h1>
      </div>
    </header>

    <main class="max-w-7xl mx-auto px-4 py-6">
      <RouterView />
    </main>

    <footer class="bg-white border-t mt-auto">
      <div class="max-w-7xl mx-auto px-4 py-4 text-center text-gray-500">
        &copy; 2025 ArchitecturePlayground
      </div>
    </footer>
  </div>
</template>
