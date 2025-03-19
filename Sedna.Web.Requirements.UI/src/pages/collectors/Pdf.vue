<template>
    <div>


    <table id="tblMainDisplay" class="tblMain" style="width:1500px;height: 500px;">
        <tr>
            <td class="tableColumnSide">
                

                <span class="smallText">Ad hoc search:</span>
                <input id="textSearchString" value="" />
                <button id="searchForString" v-on:click="btnSearchClick();">Search...</button>

                <div class="scrollable">
                <searchBar /> 
                </div>

            </td>
            <td class="tableColumnMain">

        <iframe id="viewerFrame"  v-on:load="pdfIframeLoaded();"
        src="/viewer.html?file=/pdfs/54Pager.pdf" frameborder="1" 
        style="margin: 0px;border: 4px inset lightgray; width: 900px;height: 900px;"></iframe>
    
                </td>
            <td class="tableColumnSide">
                <span>Selected Text:</span>

                <div id="selectedPdfText" class="selectedPdfText"></div>
                <div style="position:relative;">
                    <div id="clickToCapture" class="divClickToCaptureRight" v-on:click="setDetailsFormVisible(true);">
                        Capture ad hoc...</div>
                <div id="clickToCapture" class="divClickToCaptureLeft" v-on:click="setDetailsFormVisible(true);">
                    Capture this...</div>
                </div>
                <div id="divCaptureDetailsForm" class="divCaptureDetailsForm" >
                    <div style="height: 3px;background-color: lightslategrey; margin-top: 14px; margin-bottom: 14px;;"></div>
                    <div>Associate this requirement with:</div>
                    <input id="radioWholeOpportunity" type="radio" name="associatedWith" checked v-on:click="clickedWholeOppRadio();"/>Whole Opportunity, or<br />
                    <input id="radioPartLines" type="radio" name="associatedWith" v-on:click="clickedPartLineRadio();"/>Specific parts: <br />
                    <div id="divPartLineCheckboxes" class="divPartLineCheckboxes">
                        <input id="checkboxPartLine1" type="checkbox" value="[Part-Line #1]" disabled="true" />[Part-Line #1]<br />
                        <input id="checkboxPartLine1" type="checkbox" value="[Part-Line #2]" disabled="true" />[Part-Line #2]<br />
                        <input id="checkboxPartLine1" type="checkbox" value="[Part-Line #3]" disabled="true" />[Part-Line #3]<br />
                    </div>
                    <input id="hiddenOriginalText" type="hidden" value="" />
                    <div>Paraphrased (Layman's terms):</div>
                    <div id="editableSelectedText" class="editableSelectedText" contenteditable="true"></div>
                    <button id="saveRequirement" v-on:click="captureRequirement();">Capture this requirement</button>
                </div>

                <div style="height: 3px;background-color: lightslategrey; margin-top: 14px; margin-bottom: 14px;;"></div>
                    <div>Captured Requirements:</div>
                <div id="divRequirementsCaptured" class="divRequirementsCaptured"></div>

                <!-- ================================================ -->

<div id="requirementsCollection">
    <DxDataGrid
      :data-source="knownRequirements"
      :show-borders="true"
      :show-row-lines="true"
      :show-column-lines="false"
      :key-expr="requirementId"
      :row-alternation-enabled="true"
      @row-dbl-click="requirementDblClick"
      @row-updated="requirementUpdated"
      no-data-text="No requirements yet..."
    >
      <DxPaging :enabled="false"/>
      <DxEditing
        :allow-updating="true"
        :allow-deleting="true"
        :allow-adding="true"
        :use-icons="true"
        mode="popup"
        
      >
      <DxTexts 
        confirm-delete-message="Delete this requirement?"

      />
        <DxPopup
          :show-title="true"
          :width="700"
          :height="375"
          title="Add/Edit Requirement"
        >
          <DxPosition
            my="center"
            at="center"
            of="window"
          />
        </DxPopup>
        <DxForm>
            <DxItem
            :col-count="2"
            :col-span="2"
            item-type="group"            
          >
            <DxItem data-field="topic.name"  :editor-options="{readOnly: true}" />
            <DxItem data-field="sourceText"  :editor-options="{readOnly: true}" />
            <DxItem data-field="subject.description"  :editor-options="{readOnly: true}"/>
            <DxItem data-field="reviewDt"  :editor-options="{readOnly: true}" />
            <DxItem data-field="preferredPhrasing"
              :col-span="2"
              editor-type="dxTextArea"
              :editor-options="textAreaOptions"
            />

          </DxItem>
        </DxForm>
      </DxEditing>      
            
            <DxColumn                    
                    :calculate-cell-value="createRequirementSummary"
                    caption="Requirements"
                    :encode-html="false"
                />
            <DxColumn data-field="topic.name" :visible="false"/> 
            <DxColumn data-field="sourceText" :visible="false"/> 
            <DxColumn data-field="subject.description" :visible="false"/> 
            <DxColumn data-field="reviewDt" :visible="false" data-type="date"/> 
            <DxColumn data-field="preferredPhrasing" :visible="false"/> 
      
    </DxDataGrid>
  </div>

            </td>


        </tr>
    </table>




    </div>
</template>

<script>

    import EventBus from '../../event_bus';
    import axios from 'axios';

    import pdf from 'vue-pdf'
    import searchBar from '../../components/RequirementsCapture/searchBar'
    import '@fortawesome/fontawesome-free/css/all.css'
    import '@fortawesome/fontawesome-free/js/all.js'

    import {
        DxDataGrid,
        DxColumn,
        DxPaging,
        DxEditing,
        DxTexts, 
        DxPopup,
        DxLookup,
        DxPosition,
        DxForm
        } from 'devextreme-vue/data-grid';
    import { DxItem, DxSimpleItem } from 'devextreme-vue/form';
    import { requirements } from '../../test_data/testdata_sourceRequirements';
    import DxTextArea from 'devextreme-vue/text-area';

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
            DxItem, DxSimpleItem, DxTextArea,
            pdf,
            searchBar
        },
        data (){
            return{
                currentPdfPageNumber: 0,
                allCapturedRequirements: new Array(),
                knownRequirements: requirements,
                textAreaOptions: { placeholder: 'Provide desired layman\'s phrasing...', height: 100 }

            }
        },
        created: function () {
            axios.get(`http://localhost:11111/v1/sources/s3-123-456-789/requirements`)
                .then(response => {
                    this.knownRequirements = response.data
                })
                .catch(e => {
                    this.knownRequirements = null;
                    this.errors.push(e)
                })
        },
    
        mounted: function () {            
            // console.clear();
            // console.log(this.knownRequirements);
        },


        methods: {
            // When the IFrame is fully loaded, we can alert 
            // all the child controls to pre-run their 
            // searches on it.
            pdfIframeLoaded() {
                //console.log("firing EventBus.$emit");
                //var iframeText = document.getElementById('viewerFrame').contentWindow.document.body.innerText;
                //console.log("iframeText = " + iframeText);
                //console.log(document.getElementById('viewerFrame').contentWindow.allPdfText().length);
                console.log("firing the 'mounted' event");
                this.docLoaded();
                console.log(" - docLoaded() succeeded");           

                EventBus.$emit('pdf.iframe.loaded');
            },       

            btnSearchClick() {
                var searchString = document.getElementById("textSearchString").value;
                this.searchForString(searchString);
                console.log(document.getElementById('viewerFrame').contentWindow.allPdfText().length);
            },

            searchForString(searchString, isWholeWord) {
                var hitCount = document.getElementById('viewerFrame').contentWindow.findPdfText(searchString, isWholeWord);
                //alert(searchString + ', ' + isWholeWord);
                console.log(hitCount + " search hits");
            },

            termExistsInPdf(text, isWholeWord){
                var appearsCount = 0;
                var iframeText = document.getElementById('viewerFrame').contentWindow.document.body.innerText;
                if(isWholeWord){
                    appearsCount = iframeText.toLowerCase().split(text).length - 1;  // if it doesn't appear, the value is 1
                }
                return (appearsCount > 0);
            },


            docLoaded() {
                console.log("docLoaded firing");
                var selectedPdfText = document.getElementById("selectedPdfText");
                //console.log("loaded");

                var iframeWinDoc = document.getElementById('viewerFrame').contentWindow.document;
                //console.log(iframeWinDoc);

                iframeWinDoc.onselectionchange = () => {
                    //var selection = iframeWinDoc.getSelection();
                    //console.log("Ranges: " + selection.getRangeAt(0).toString());
                    //console.log("IFrame: " + iframeWinDoc.getSelection().getRangeAt(0));
                    var originalText = iframeWinDoc.getSelection().toString();
                    selectedPdfText.innerText = originalText;
                    document.getElementById('hiddenOriginalText').value = originalText;
                    document.getElementById('editableSelectedText').innerText = originalText;

                    //get current page number
                    this.currentPdfPageNumber = document.getElementById('viewerFrame').contentWindow.PDFViewerApplication.pdfViewer.currentPageNumber;
                }
            },

            refreshCapturedRequirements(){
                var divCapdReqs = document.getElementById('divRequirementsCaptured');
                
                // clear all children
                while (divCapdReqs.firstChild) {
                    divCapdReqs.removeChild(divCapdReqs.firstChild);
                }

                this.allCapturedRequirements.forEach(req => {
                    var divReq = document.createElement("div");
                    divReq.className = "capturedRequirement";
                    var reqTitle = document.createElement("div");
                    reqTitle.innerText = req.editedText.split("\n")[0];
                    reqTitle.className = "capturedReq_title";
                    reqTitle.title = req.editedText;

                    var reqText = document.createElement("div");
                    reqText.className = "capturedReq_text";
                    reqText.innerText = "From page " + req.pageNumber;
                    if(req.opportunityOrLines == "lines")
                        reqText.innerHTML += "<br />Associated with " + req.associatedLines.length + " opportunity lines";
                    else
                        reqText.innerHTML += "<br />Associated with whole opportunity";


                    var deleteIcon = document.createElement("span");
                    deleteIcon.innerText = "x";
                    deleteIcon.className = "deleteReqIcon";
                    deleteIcon.title = "Delete this requirement";
                    deleteIcon.onclick = this.deleteRequirement(req);

                    // assemble whole div
                    divReq.appendChild(deleteIcon);
                    divReq.appendChild(reqTitle);
                    divReq.appendChild(reqText);

                    divCapdReqs.appendChild(divReq);
                });
            },
            deleteRequirement(req) {
                console.log(req);
            },

            setDetailsFormVisible(trueOrFalse) {
                var form = document.getElementById('divCaptureDetailsForm');
                if(trueOrFalse) {
                    form.style.display = "block";
                } 
                else {
                    form.style.display = "none";
                }
            },

            clickedWholeOppRadio() {
                var checkboxes = document.getElementById('divPartLineCheckboxes').getElementsByTagName("input");
                var inputChecks = Array.prototype.slice.call(checkboxes);
                inputChecks.forEach(chk => {
                    chk.checked = false;
                    chk.disabled = true;
                });
            },

            clickedPartLineRadio() {
                var partCheckboxes = document.getElementById('divPartLineCheckboxes').getElementsByTagName("input");
                var partInputs = Array.prototype.slice.call(partCheckboxes);
                partInputs.forEach(chk => {
                    chk.checked = false;
                    chk.disabled = false;
                });      
            },

            newRequirementObject() {
                var req = {
                    originalText:  "",
                    editedText: "",
                    opportunityOrLines: "opportunity",
                    associatedLines: new Array(),
                    source: "SPE5EY-19-R-0011",
                    pageNumber: this.currentPdfPageNumber
                };
                return req;
            },

            captureRequirement() {
                var newReq = this.newRequirementObject();
                var originalText = document.getElementById('hiddenOriginalText').value;
                var editedText = document.getElementById('editableSelectedText').innerText;

                newReq.originalText = originalText;
                newReq.editedText = editedText;

                var oppOrLines = document.getElementById('radioWholeOpportunity').checked;
                newReq.opportunityOrLines = oppOrLines ? "opportunity" : "lines" ;

                if(oppOrLines == false)
                {
                    var partCheckboxes = document.getElementById('divPartLineCheckboxes').getElementsByTagName("input");
                    var partInputs = Array.prototype.slice.call(partCheckboxes);
                    partInputs.forEach(chk => {
                        if(chk.checked) { newReq.associatedLines.push(chk.value); }
                    });
                }
                newReq.source = "Solicitation PDF";
                newReq.source = "Page XX";

                console.log(newReq);

                this.allCapturedRequirements.push(newReq);
                this.refreshCapturedRequirements();
                this.setDetailsFormVisible(false);
            },

            createRequirementSummary(requirement) {
                //console.log(requirement);
                var result = `<div style='font-size: 70%'>Topic: ${requirement.topic.name} (${requirement.sourceTextLocation})</div>
                    <div style='font-size: 90%'>&ldquo;${requirement.preferredPhrasing}&rdquo;</div>`;

                return result;

            },

            requirementDblClick(e) {                
                e.component.editRow(e.rowIndex);
            },

            requirementUpdated(data) {
                //console.log(data);
                this.updateRequirement(data.data);
            },

            updateRequirement(requirement) {
                //console.log(JSON.stringify(requirement));
                //console.log(requirement);

                axios.put(`http://localhost:11111/v1/requirements/${requirement.requirementId}`, 
                    requirement
                )
                .then(function (response) {
                    console.log("PUT to server succeeded");
                    console.log(response);
                })
                .catch(e => {
                    alert("wham! trouble!");
                    console.log(e);
                    //this.errors.push(e)
                })
            }
        }
    }


</script>


<style>
        .scrollable {
            overflow-y: scroll;
            height: 90%;
        }

        .tableColumnMain {
            width: 60%;
            vertical-align: top;
        }

        .tableColumnSide {
            width: 20%;
            vertical-align: top;
            font-size: 80%;
            background-color: whitesmoke;
            border: lightgray 1px solid;
            padding: 8px;
        }

        * {
            font-family: Verdana, Helvetica, sans-serif;
        }
        
        .knownSearchButton {
            display: block;
            width: 300px;
            position: relative;
            margin-bottom: 4px;
            padding-right: 20px;
        }

        .smallText {
            padding-top: 4px;
            display: block; 
        }

        .tblMain tr td {
            padding-bottom: 12px;
        }

        .columnHeader {
            background-color: #eeeeee;
            padding: 3px;
            border: solid 1px #bbbbbb;
            height: 38px;
            vertical-align: text-bottom;
        }

        .selectedPdfText {
            padding: 5px; 
            padding-bottom: 0px;
            font-family: 'Courier New', Courier, monospace; 
            font-size: 90%; 
            border: 1px solid #bbb; 
            margin-bottom: 8px;
            max-height: 200px; 
            overflow-y: auto;
            background-color: #f9f9f9;
        }

        .divClickToCaptureLeft {
            margin-top: 0px;
            margin-bottom: 10px;
            color: blue;
            text-decoration: underline;
            text-align: left;
            cursor: pointer;
            display: inline-block;
 }

        .divClickToCaptureRight {
            margin-top: 0px;
            margin-bottom: 10px;
            color: blue;
            text-decoration: underline;
            text-align: right;
            cursor: pointer;
            position: absolute;
            right: 10px;
            display: inline-block;
      }

        .divPartLineCheckboxes {
            padding:9px;
        }

        .divCaptureDetailsForm {
            display: none;
        }

        .editableSelectedText {
            padding: 5px; 
            padding-bottom: 0px;
            font-family: 'Courier New', Courier, monospace; 
            font-size: 90%; 
            border: 1px solid #bbb; 
            margin-bottom: 8px;
            max-height: 200px; 
            overflow-y: auto;
            background-color: #fff;
        }

        .divRequirementsCaptured {
            padding: 7px;
            padding-left: 14px;
        }

        .capturedRequirement {
            margin-bottom: 12px;
            border-bottom: dashed 1px grey;
            position: relative;
        }

        .capturedReq_title {
            /* get the ellipsis effect */
            white-space: nowrap;
            overflow-x: hidden;
            text-overflow: ellipsis;
            width: 230px;

            /* styling */
            font-weight: bold;
            color: #444444;
            font-size: 90%;
    }
    
        .capturedReq_text {
            padding-left: 8px;   
            font-size: 85%;
        }

        .deleteReqIcon {
            color: darkred;
            font-size: 85%;
            font-weight: bold;
            position: absolute;
            top: 0px;
            right: 0px;
            width: 15px;
            cursor: pointer;
        }



        svg.red {
            color: grey;
            margin-right: 7px;
            position: absolute;
            right: 5px;
            top: 3px;
    }

        svg.green {
            color: rgb(1, 167, 1);
            margin-right: 7px;
            position: absolute;
            right: 5px;
            top: 3px;
        }
</style>