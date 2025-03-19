<template>
    <div v-if="topics && topics.length"  >
        <div class="topicHeader" v-for="topic of topics" :key="topic.topicId">
            <searchBarTopic v-bind:topic="topic" />
        </div>
    </div>
</template>
 
<script>
    import searchBarTopic from './searchBarTopic'; 
    import axios from 'axios';

    export default {
        components: {searchBarTopic},
 
        data() {
            return {
                topics: [],
                errors: []
            }
        },
 

        // Fetches posts when the component is created.
        created() {
            axios.get(`http://localhost:11111/v1/subjecttypes/1/topics`)
            .then(response => {
            // JSON responses are automatically parsed.
            this.topics = response.data
            })
            .catch(e => {
            this.errors.push(e)
            })
        },
        methods: {
            searchForString(text, isWholeWord) {
                this.$parent.searchForString(text, isWholeWord);
            },

            termExistsInPdf(text, isWholeWord){
                return this.$parent.termExistsInPdf(text, isWholeWord);
            }
        }
    }
    
</script>
 
<style scoped>

    .topicHeader {
        margin-top: 12px;
    }
</style> 