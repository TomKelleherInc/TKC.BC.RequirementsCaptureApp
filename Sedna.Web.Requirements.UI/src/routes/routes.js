import PlatformLayout from '@/layouts/Platform';
import MinimalLayout from '@/layouts/Minimal';


const authRoutes = {
    path: '/auth',
    component: MinimalLayout,
    children: [{
        path: 'login',
        component: () => import('@/pages/auth/Login.vue')
    }]
};

const collectorsRoutes = {
    path: '/collectors',
    component: PlatformLayout,
    children: [
        {
            path: 'pdf',
            component: () => import('@/pages/collectors/Pdf.vue')
        }
    ]
};
 
const adminRoutes = {
    path: '/admin',
    component: PlatformLayout,
    children: [
        {
            path: 'admin',
            component: () => import('@/pages/admin/admin.vue')
        }
    ]
};


export const routes = [
    // { path: '', redirect: '/dashboards/dashboard-1' },
    authRoutes,
    collectorsRoutes,
    adminRoutes
];

