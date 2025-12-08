<script setup lang="ts">
import { ref } from 'vue'

// Placeholder products
const products = ref([
  { id: 1, name: 'Product 1', price: 99.99, image: 'https://via.placeholder.com/300' },
  { id: 2, name: 'Product 2', price: 149.99, image: 'https://via.placeholder.com/300' },
  { id: 3, name: 'Product 3', price: 199.99, image: 'https://via.placeholder.com/300' },
  { id: 4, name: 'Product 4', price: 79.99, image: 'https://via.placeholder.com/300' },
  { id: 5, name: 'Product 5', price: 129.99, image: 'https://via.placeholder.com/300' },
  { id: 6, name: 'Product 6', price: 249.99, image: 'https://via.placeholder.com/300' },
])

const addToCart = (productId: number) => {
  // TODO: Implement cart logic
}
</script>

<template>
  <div class="container mx-auto px-4 py-8">
    <h1 class="text-3xl font-bold text-gray-900 mb-8">Products</h1>

    <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
      <div
        v-for="product in products"
        :key="product.id"
        class="bg-white rounded-lg shadow-md overflow-hidden hover:shadow-lg transition"
      >
        <img
          :src="product.image"
          :alt="product.name"
          class="w-full h-48 object-cover"
        />
        <div class="p-4">
          <h3 class="text-lg font-semibold text-gray-900">{{ product.name }}</h3>
          <p class="text-gray-600 mt-1">${{ product.price.toFixed(2) }}</p>
          <button
            @click="addToCart(product.id)"
            class="mt-4 w-full bg-blue-600 text-white py-2 px-4 rounded hover:bg-blue-700 transition"
          >
            Add to Cart
          </button>
        </div>
      </div>
    </div>
  </div>
</template>
