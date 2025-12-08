import { createRouter, createWebHistory } from 'vue-router'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: () => import('@/modules/catalog/views/HomeView.vue')
    },
    {
      path: '/login',
      name: 'login',
      component: () => import('@/modules/identity/views/LoginView.vue')
    },
    {
      path: '/register',
      name: 'register',
      component: () => import('@/modules/identity/views/RegisterView.vue')
    },
    {
      path: '/products',
      name: 'products',
      component: () => import('@/modules/catalog/views/ProductsView.vue')
    },
    {
      path: '/cart',
      name: 'cart',
      component: () => import('@/modules/basket/views/CartView.vue')
    },
    {
      path: '/orders',
      name: 'orders',
      component: () => import('@/modules/ordering/views/OrdersView.vue'),
      meta: { requiresAuth: true }
    }
  ]
})

export default router
