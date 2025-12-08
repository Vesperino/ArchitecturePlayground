<script setup lang="ts">
// Home page component
</script>

<template>
  <div class="container mx-auto px-4 py-8">
    <h1 class="text-4xl font-bold text-gray-900 mb-4">
      Welcome to ArchitecturePlayground
    </h1>
    <p class="text-lg text-gray-600 mb-8">
      E-Commerce Platform demonstrating enterprise architecture patterns
    </p>

    <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
      <div class="bg-white rounded-lg shadow-md p-6">
        <h2 class="text-xl font-semibold mb-2">Modular Monolith</h2>
        <p class="text-gray-600">Clean architecture with vertical slices</p>
      </div>
      <div class="bg-white rounded-lg shadow-md p-6">
        <h2 class="text-xl font-semibold mb-2">.NET 9 + Vue 3</h2>
        <p class="text-gray-600">Modern tech stack with TypeScript</p>
      </div>
      <div class="bg-white rounded-lg shadow-md p-6">
        <h2 class="text-xl font-semibold mb-2">Cloud Native</h2>
        <p class="text-gray-600">PostgreSQL, MongoDB, Redis, RabbitMQ</p>
      </div>
    </div>

    <div class="mt-8">
      <router-link
        to="/products"
        class="inline-block bg-blue-600 text-white px-6 py-3 rounded-lg hover:bg-blue-700 transition"
      >
        Browse Products
      </router-link>
    </div>
  </div>
</template>
