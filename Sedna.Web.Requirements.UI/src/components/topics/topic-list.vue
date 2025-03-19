<template>
  <div>
    <h2>Topics List</h2>
    <DxDataGrid
      id="gridContainer"
      :ref="dataGridRefName"
      :data-source="topicsList"
      :show-borders="true"
      @row-inserted="topicAdded"
      @row-updated="topicUpdated"
    >

    <DxPaging :enabled="true"/>
      <DxEditing
        :allow-updating="true"
        :allow-deleting="false"
        :allow-adding="true"
        mode="row"
      />

      <DxFilterRow
        :visible="showFilterRow"
      />
      <DxHeaderFilter
        :visible="showHeaderFilter"
      />
      <DxSearchPanel
        :visible="true"
        :width="240"
        placeholder="Search..."
      />
      
      <DxColumn data-field="name"
        :header-filter="{ allowSearch: true }"
        caption="Name"
      />
      <DxColumn
        :header-filter="{ allowSearch: true }"
        data-field="description"
        caption="Description"
      />
      <DxColumn
        :header-filter="{ allowSearch: true }"
        data-field="preferredPhrasing"
        caption="Preferred Phrasing"
      />    </DxDataGrid>
  </div>
</template>

<script>

    //import EventBus from '../../event_bus';
    import axios from 'axios';

    import {
        DxDataGrid,
        DxColumn,
        DxPaging,
        DxEditing,
        DxTexts, 
        DxPopup,
        DxLookup,
        DxPosition,
        DxForm,
        DxSearchPanel,
        DxHeaderFilter,
        DxFilterRow
        } from 'devextreme-vue/data-grid';

    import { DxItem, DxSimpleItem } from 'devextreme-vue/form';
    import DxTextArea from 'devextreme-vue/text-area';

    import { topicsList } from '../../test_data/topicsList';


export default {
    components: {
            DxDataGrid,
            DxColumn,
            DxPaging,
            DxEditing,
            DxTexts,
            DxPopup,
            DxLookup,
            DxPosition,
            DxForm,
            DxSearchPanel, DxHeaderFilter, DxFilterRow,
            DxItem, DxSimpleItem, DxTextArea,
        },
        data (){
            return{
                showHeaderFilter: true,
                showFilterRow: true,
                topicsList: topicsList,
                dataGridRefName: 'dataGrid'
            }
        },
        created: function () {
            axios.get(`http://localhost:11111/v1/topics`)
                .then(response => {
                    this.topicsList = response.data
                })
                .catch(e => {
                    this.topicsList = null;
                    this.errors.push(e)
                })
        },
        methods: {
          topicAdded(data) {
            var topicData = data.data;

            axios.post(`http://localhost:11111/v1/topics`, 
                    topicData
                )
                .then(function (response) {
                    console.log("POST to server succeeded");
                    console.log(response);
                    alert('new topic added');
                })
                .catch(e => {
                    console.log(e);
                    alert("wham! trouble adding topic!");
                    //this.errors.push(e)
                })
          },          
          topicUpdated(data) {
            var topicData = data.data;

            axios.put(`http://localhost:11111/v1/topics/${topicData.topicId}`, 
                    topicData
                )
                .then(function (response) {
                    //alert('just updated');
                    console.log("PUT to server succeeded");
                    console.log(response);
                })
                .catch(e => {
                    //alert("wham! trouble!");
                    console.log(e);
                    //this.errors.push(e)
                })
          }

        }

}
</script>

<style scoped>

</style>