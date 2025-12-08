<script setup lang="ts">
import { ref } from 'vue'

// Placeholder orders
const orders = ref([
  {
    id: 'ORD-001',
    date: '2025-01-08',
    status: 'Delivered',
    total: 249.98,
    items: 3
  },
  {
    id: 'ORD-002',
    date: '2025-01-05',
    status: 'Processing',
    total: 149.99,
    items: 1
  },
])

const getStatusColor = (status: string) => {
  switch (status) {
    case 'Delivered': return 'bg-green-100 text-green-800'
    case 'Processing': return 'bg-yellow-100 text-yellow-800'
    case 'Shipped': return 'bg-blue-100 text-blue-800'
    case 'Cancelled': return 'bg-red-100 text-red-800'
    default: return 'bg-gray-100 text-gray-800'
  }
}
</script>

<template>
  <div class="container mx-auto px-4 py-8">
    <h1 class="text-3xl font-bold text-gray-900 mb-8">My Orders</h1>

    <div v-if="orders.length === 0" class="text-center py-12">
      <p class="text-gray-600 text-lg">You haven't placed any orders yet</p>
      <router-link
        to="/products"
        class="inline-block mt-4 bg-blue-600 text-white px-6 py-2 rounded hover:bg-blue-700"
      >
        Start Shopping
      </router-link>
    </div>

    <div v-else class="space-y-4">
      <div
        v-for="order in orders"
        :key="order.id"
        class="bg-white p-6 rounded-lg shadow"
      >
        <div class="flex flex-wrap items-center justify-between gap-4">
          <div>
            <p class="font-semibold text-gray-900">{{ order.id }}</p>
            <p class="text-sm text-gray-600">{{ order.date }}</p>
          </div>
          <div class="text-center">
            <p class="text-sm text-gray-600">Items</p>
            <p class="font-semibold">{{ order.items }}</p>
          </div>
          <div class="text-center">
            <p class="text-sm text-gray-600">Total</p>
            <p class="font-semibold">${{ order.total.toFixed(2) }}</p>
          </div>
          <div>
            <span
              :class="getStatusColor(order.status)"
              class="px-3 py-1 rounded-full text-sm font-medium"
            >
              {{ order.status }}
            </span>
          </div>
          <button class="text-blue-600 hover:text-blue-800 font-medium">
            View Details
          </button>
        </div>
      </div>
    </div>
  </div>
</template>
