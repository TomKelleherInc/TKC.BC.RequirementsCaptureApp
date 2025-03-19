<template>
    <div>
        <div class="searchBarTopicSearch" :class="[foundInPdf ? 'foundInPdf' : 'notFoundInPdf']" v-on:click="searchForString(topicSearch.searchString, topicSearch.isWholeWord);" >&ldquo;{{topicSearch.searchString}}&rdquo;</div>        
    </div>
</template>
 
<script>

    // Import the EventBus.
    import EventBus from '../../event_bus';
    

    export default {

        props: {
            topicSearch: {
                type: Object
            },
            divClass: {
                type: String,
                default: "notFoundInPdf"
            },
            foundInPdf: {
                type: Boolean,
                default: false
            }
        },
        
        methods: {
            searchForString(text, isWholeWord) {
                this.$parent.searchForString(text, isWholeWord);
            },
            preSearchThisString(){

                // var location = -1;
                // location = iframeText.indexOf(this.topicSearch.string);  // -1 if doesn't appear
                // console.log(this.topicSearch.string + ": " + location);

                // if (location > -1){
                //    this.foundInPdf = true;
                //    console.log("iframe HTML contains " + this.topicSearch.string);                
                // }
                // else {
                //    this.foundInPdf = false;
                // }
            }
        },
        mounted: function () {
            // Listen for the 'pdf.iframe.loaded' event.
            //console.log("created");
            //console.log("EventBus is " + EventBus);
            EventBus.$on('pdf.iframe.loaded', this.preSearchThisString); 

 
        },
        // mounted: function () {
        //      console.log("THIS TOPIC STRING = " + this.topicSearch.string);
        // }, 
    }
    
</script>
 
<style scoped>
    .searchBarTopicSearch {
        width: 80%;
        margin-left: 12px;
        padding: 2px;
        padding-left: 7px;
        cursor: pointer;
        background-color: whitesmoke;
        margin-bottom: 4px;
        color: #222222;
    }
    .searchBarTopicSearch:hover {
        background-color: #cddeef;
    }

    .foundInPdf {
        background-color: lightgreen;
        color: black;
    }

    .notFoundInPdf {
        background-color: lightgray;
        color: #999999;
    }

</style>