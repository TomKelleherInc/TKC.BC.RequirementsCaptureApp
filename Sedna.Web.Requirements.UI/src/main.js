import 'devextreme/dist/css/dx.common.css';
import 'devextreme/dist/css/dx.light.css';
import Vue from 'vue';
import App from './App.vue';
import store from './store'
import VueRouter from 'vue-router';
import { routes } from './routes/routes';
import { index } from './components/index';

Vue.use(VueRouter);

const router = new VueRouter({
    routes,
    linkActiveClass: 'open active',
    scrollBehavior: () => ({ y: 0 }),
    mode: 'hash'
});

Vue.config.productionTip = true;

new Vue({
    router,
    store,
    render: h => h(App)
}).$mount('#app');

