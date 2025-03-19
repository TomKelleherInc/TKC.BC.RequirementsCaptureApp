/*
An event-bus to help communicate information from one part of 
the app to other parts.  Parts that want to send an event out
use the ".emit" method with a custom event key, and others listen 
for that key using the ".on" method.

https://alligator.io/vuejs/global-event-bus/

*/

import Vue from 'vue'
export default new Vue()