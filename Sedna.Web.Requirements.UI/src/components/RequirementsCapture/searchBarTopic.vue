<template>
    <div>
        <div class="sidebarTopic" >{{topic.name}}</div>
        <div class="searchBar-loop" v-for="topicSearch of topic.topicSearch" :key="topicSearch.topicSearchId">
            <searchBarTopicSearch v-bind:topicSearch="topicSearch" />
        </div>
    </div>
</template>
 
<script>
    import searchBarTopicSearch from './searchBarTopicSearch'; 

    export default {
        components: {searchBarTopicSearch},

        props: {
            topic: {
                type: Object
            }
        },
        
        methods: {
            searchForString(text, isWholeWord) {
                this.$parent.searchForString(text, isWholeWord);
            },

            termExistsInPdf(text, isWholeWord){
                return this.$parent.termExistsInPdf(text, isWholeWord);
            }
        },
        mounted: function () {
             //console.log("THIS TOPIC = " + this.topic.name);
        }, 
    }
</script>
 
<style scoped>
    .sidebarTopic {
        width: 300px;
        border-bottom: 1px solid #dddddd;

    }
</style>