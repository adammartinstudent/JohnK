$w.onReady(function () {
    /*
    *   Unable to connect to foreign key.
    *   Further research necessary.
    *   Javascript weird and scary.
    */
    $w("#table1").columns = [{
        "id": "col1", // Field
        "dataPath": "intake", // value
        "label": "Intake #", // alias
        "type": "int", // data type
    }, {
        "id": "col2",
        "dataPath": "name",
        "label": "Name",
        "type": "string",
    }, {
        "id": "col3",
        "dataPath": "species",
        "label": "Species",
        "type": "string",
    }, {
        "id": "col4",
        "dataPath": "breed",
        "label": "Breed",
        "type": "string",
    }, {
        "id": "col5",
        "dataPath": "color",
        "label": "Color",
        "type": "string",
    }, {
        "id": "col6",
        "dataPath": "weight",
        "label": "Weight",
        "type": "int",
    }, {
        "id": "col7",
        "dataPath": "gender",
        "label": "Gender",
        "type": "char",
    }, {
        "id": "col8",
        "dataPath": "altered",
        "label": "Altered",
        "type": "bool",
    }, {
        "id": "col9",
        "dataPath": "microchipped",
        "label": "Microchipped",
        "type": "bool",
    }];
});
