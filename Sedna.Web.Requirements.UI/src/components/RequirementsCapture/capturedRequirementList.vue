<template>
    <div>
        <div class="">Captured Requirements</div>
        <div v-for="requirement of requirements" :key="requirement.requirementId">
            <capturedRequirement v-bind="requirement" />
            
    </div>
</template>

<script>
import capturedRequirement from "./capturedRequirement"

export default {
    components: {capturedRequirement},
        props: {
            requirement: {
                type: Object
            }
        },
    
}
</script>