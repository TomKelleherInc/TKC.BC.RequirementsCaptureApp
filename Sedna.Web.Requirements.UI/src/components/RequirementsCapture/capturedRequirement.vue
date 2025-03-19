<template>
    
    <div>
        <div class="capturedRequirement">
            <span class="deleteReqIcon" title="Delete this requirement">x</span>
            <div class="capturedReq_title" v-bind:title="requirement.preferredPhrasing">
                {{requirement.preferredPhrasing}}
            </div>
            <div class="capturedReq_text">
                From page {{requirement.sourceTextLocation}}
                <br>
                {{this}}
            </div>
        </div>
    </div>

</template>

<script>
/*
-- a Requirement object in Json
{
    "requirementId": 1,
    "sourceId": 1,
    "sourceText": "This is hazmat",
    "sourceTextLocation": "page:31",
    "preferredPhrasing": "This part is considered hazardous material",
    "topicId": 2,
    "subjectExternalId": 12345,
    "entityId": 1,
    "isActive": true,
    "reviewDt": null,
    "createdTs": "2020-02-11T18:01:44.107",
    "updatedTs": "2020-02-11T18:01:44.107",
    "createdUserId": 529,
    "updatedUserId": 529,
    "entity": null,
    "source": null,
    "topic": null,
    "requirementContext": []
}

*/

export default {
    
}
</script>


<style scoped>

</style>