<script setup lang="ts">
import { ref, computed } from 'vue'

// Placeholder cart items
const cartItems = ref([
  { id: 1, name: 'Product 1', price: 99.99, quantity: 2 },
  { id: 2, name: 'Product 2', price: 149.99, quantity: 1 },
])

const total = computed(() =>
  cartItems.value.reduce((sum, item) => sum + item.price * item.quantity, 0)
)

const updateQuantity = (id: number, quantity: number) => {
  const item = cartItems.value.find(i => i.id === id)
  if (item && quantity > 0) {
    item.quantity = quantity
  }
}

const removeItem = (id: number) => {
  cartItems.value = cartItems.value.filter(i => i.id !== id)
}

const checkout = () => {
  console.log('Proceeding to checkout')
  // TODO: Implement checkout logic
}
</script>

<template>
  <div class="container mx-auto px-4 py-8">
    <h1 class="text-3xl font-bold text-gray-900 mb-8">Shopping Cart</h1>

    <div v-if="cartItems.length === 0" class="text-center py-12">
      <p class="text-gray-600 text-lg">Your cart is empty</p>
      <router-link
        to="/products"
        class="inline-block mt-4 bg-blue-600 text-white px-6 py-2 rounded hover:bg-blue-700"
      >
        Continue Shopping
      </router-link>
    </div>

    <div v-else class="grid grid-cols-1 lg:grid-cols-3 gap-8">
      <div class="lg:col-span-2">
        <div
          v-for="item in cartItems"
          :key="item.id"
          class="flex items-center justify-between bg-white p-4 rounded-lg shadow mb-4"
        >
          <div class="flex-1">
            <h3 class="font-semibold text-gray-900">{{ item.name }}</h3>
            <p class="text-gray-600">${{ item.price.toFixed(2) }}</p>
          </div>
          <div class="flex items-center gap-4">
            <input
              type="number"
              :value="item.quantity"
              @change="updateQuantity(item.id, Number(($event.target as HTMLInputElement).value))"
              min="1"
              class="w-16 px-2 py-1 border rounded text-center"
            />
            <button
              @click="removeItem(item.id)"
              class="text-red-600 hover:text-red-800"
            >
              Remove
            </button>
          </div>
        </div>
      </div>

      <div class="bg-white p-6 rounded-lg shadow h-fit">
        <h2 class="text-xl font-semibold mb-4">Order Summary</h2>
        <div class="flex justify-between mb-2">
          <span class="text-gray-600">Subtotal</span>
          <span>${{ total.toFixed(2) }}</span>
        </div>
        <div class="flex justify-between mb-4">
          <span class="text-gray-600">Shipping</span>
          <span>Free</span>
        </div>
        <hr class="my-4" />
        <div class="flex justify-between text-lg font-semibold mb-6">
          <span>Total</span>
          <span>${{ total.toFixed(2) }}</span>
        </div>
        <button
          @click="checkout"
          class="w-full bg-blue-600 text-white py-3 rounded-lg hover:bg-blue-700 transition"
        >
          Proceed to Checkout
        </button>
      </div>
    </div>
  </div>
</template>
