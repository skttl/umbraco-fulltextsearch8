angular.module("umbraco")
    .controller("Our.Umbraco.FullTextSearch.Dashboard.SearchAdvancedSettingsController",
        [
            "$scope",
            "Our.Umbraco.FullTextSearch.Resource",
            function ($scope, fullTextSearchResource) {

                var vm = this;

                /*
                 * Localize keys
                 */
                vm.dictionaryKeys = fullTextSearchResource.localizeKeys({
                    fullTextSearch_searchType: "Search type",
                    fullTextSearch_searchTypeDescription: "The type of search to perform.",
                    fullTextSearch_titleProperties: "Title properties",
                    fullTextSearch_titlePropertiesDescription: "Adds field names to use for title properties. Note, that this overrides the config setting, so you need to add all wanted fields for titles here.",
                    fullTextSearch_titleBoost: "Title boost",
                    fullTextSearch_titleBoostDescription: "Set the boosting value for the title properties, to make titles more important than body text when searching.",
                    fullTextSearch_bodyProperties: "Body properties",
                    fullTextSearch_bodyPropertiesDescription: "Adds field names to use for body properties. Note, that this overrides the config setting, so you need to add all wanted fields for bodytext here.",
                    fullTextSearch_summaryProperties: "Summary properties",
                    fullTextSearch_summaryPropertiesDescription: "Adds field names to use for summary properties. Note, that if you don't specify any summary properties, the body properties will be used instead.",
                    fullTextSearch_summaryLength: "Summary length",
                    fullTextSearch_summaryLengthDescription: "Sets the summary length in the results. The default is 300 characters.",
                    fullTextSearch_rootNodes: "Root nodes",
                    fullTextSearch_rootNodesDescription: "With this setting, you can limit search results to be descendants of the selected nodes.",
                    fullTextSearch_culture: "Culture",
                    fullTextSearch_cultureDescription: "This is used to define which culture to search in. You should probably always set this, but it might work without it, in invariant sites.",
                    fullTextSearch_enableWildcards: "Enable wildcards",
                    fullTextSearch_enableWildcardsDescription: "These enables or disables use of wildcards in the search terms. Wildcard characters are added automatically to each of the terms.",
                    fullTextSearch_fuzzyness: "Fuzzyness",
                    fullTextSearch_fuzzynessDescription: "Fuzzyness is used to match your search term with similar words. This method sets the fuzzyness parameter of the search. The default is 0.8. If wildcards is enabled, fuzzyness will not be used."
                });

                var searchTypes = {};
                ["MultiRelevance", "MultiAnd", "SimpleOr", "AsEntered"].map((value, i) => {
                    searchTypes[i] = {
                        "value": value,
                        "sortOrder": i
                    };
                });

                var validation = {
                    "mandatory": false,
                    "mandatoryMessage": null,
                    "pattern": null,
                    "patternMessage": null
                };

                vm.properties = [
                    {
                        "label": vm.dictionaryKeys.fullTextSearch_searchType,
                        "description": vm.dictionaryKeys.fullTextSearch_searchTypeDescription,
                        "view": "dropdownFlexible",
                        "config": {
                            "items": searchTypes,
                            "multiple": false
                        },
                        "id": -1,
                        "value": $scope.model.advancedSettings.searchType.toString(),
                        "alias": "culture",
                        "editor": "Umbraco.DropDown.Flexible",
                        "validation": validation
                    },
                    {
                        "label": vm.dictionaryKeys.fullTextSearch_titleProperties,
                        "description": vm.dictionaryKeys.fullTextSearch_titlePropertiesDescription,
                        "view": "multipletextbox",
                        "config": {
                            "min": 1,
                            "max": 0
                        },
                        "id": 0,
                        "value": $scope.model.advancedSettings.titleProperties.map(p => { return { value: p } }),
                        "alias": "titleProperties",
                        "editor": "Umbraco.MultipleTextstring",
                        "validation": validation
                    },

                    {
                        "label": vm.dictionaryKeys.fullTextSearch_titleBoost,
                        "description": vm.dictionaryKeys.fullTextSearch_titleBoostDescription,
                        "view": "decimal",
                        "config": {
                            "min": 0,
                            "step": 0.1,
                            "max": null
                        },
                        "hideLabel": false,
                        "id": 8,
                        "value": $scope.model.advancedSettings.titleBoost.toString(),
                        "alias": "titleBoost",
                        "editor": "Umbraco.Decimal",
                        "validation": validation
                    },
                    {
                        "label": vm.dictionaryKeys.fullTextSearch_bodyProperties,
                        "description": vm.dictionaryKeys.fullTextSearch_bodyPropertiesDescription,
                        "view": "multipletextbox",
                        "config": {
                            "min": 1,
                            "max": 0
                        },
                        "id": 1,
                        "value": $scope.model.advancedSettings.bodyProperties.map(p => { return { value: p } }),
                        "alias": "bodyProperties",
                        "editor": "Umbraco.MultipleTextstring",
                        "validation": validation
                    },
                    {
                        "label": vm.dictionaryKeys.fullTextSearch_summaryProperties,
                        "description": vm.dictionaryKeys.fullTextSearch_summaryPropertiesDescription,
                        "view": "multipletextbox",
                        "config": {
                            "min": 1,
                            "max": 0
                        },
                        "id": 2,
                        "value": $scope.model.advancedSettings.summaryProperties.map(p => { return { value: p } }),
                        "alias": "summaryProperties",
                        "editor": "Umbraco.MultipleTextstring",
                        "validation": validation
                    },
                    {
                        "label": vm.dictionaryKeys.fullTextSearch_summaryLength,
                        "description": vm.dictionaryKeys.fullTextSearch_summaryLengthDescription,
                        "view": "integer",
                        "config": {
                            "min": 0,
                            "step": 1,
                            "max": null
                        },
                        "hideLabel": false,
                        "id": 3,
                        "value": $scope.model.advancedSettings.summaryLength,
                        "alias": "summaryLength",
                        "editor": "Umbraco.Integer",
                        "validation": validation
                    },
                    {
                        "label": vm.dictionaryKeys.fullTextSearch_rootNodes,
                        "description": vm.dictionaryKeys.fullTextSearch_rootNodesDescription,
                        "view": "contentPicker",
                        "config": {
                            "ignoreUserStartNodes": false,
                            "maxNumber": 0,
                            "minNumber": 0,
                            "multiPicker": true,
                            "showOpenButton": false,
                            "startNode": null
                        },
                        "hideLabel": false,
                        "id": 4,
                        "value": $scope.model.advancedSettings.rootNodeIds.join(","),
                        "alias": "rootNodeIds",
                        "editor": "Umbraco.MultiNodeTreePicker",
                        "validation": validation
                    },
                    {
                        "label": vm.dictionaryKeys.fullTextSearch_culture,
                        "description": vm.dictionaryKeys.fullTextSearch_cultureDescription,
                        "view": "dropdownFlexible",
                        "config": {
                            "items": $scope.model.options.cultures,
                            "multiple": false
                        },
                        "id": 5,
                        "value": $scope.model.advancedSettings.culture ?? cultures[0].value,
                        "alias": "culture",
                        "editor": "Umbraco.DropDown.Flexible",
                        "validation": validation
                    },
                    {
                        "label": vm.dictionaryKeys.fullTextSearch_enableWildcards,
                        "description": vm.dictionaryKeys.fullTextSearch_enableWildcardsDescription,
                        "view": "boolean",
                        "config": {
                            "default": null,
                            "labelOn": null
                        },
                        "hideLabel": false,
                        "id": 6,
                        "value": $scope.model.advancedSettings.enableWildcards ? "1" : "0",
                        "alias": "enableWildcards",
                        "editor": "Umbraco.TrueFalse",
                        "validation": validation
                    },

                    {
                        "label": vm.dictionaryKeys.fullTextSearch_fuzzyness,
                        "description": vm.dictionaryKeys.fullTextSearch_fuzzynessDescription,
                        "view": "decimal",
                        "config": {
                            "min": 0,
                            "step": 0.1,
                            "max": 1
                        },
                        "hideLabel": false,
                        "id": 7,
                        "value": $scope.model.advancedSettings.fuzzyness,
                        "alias": "fuzzyness",
                        "editor": "Umbraco.Decimal",
                        "validation": validation
                    },
                ]

                vm.submit = submit;
                vm.close = close;

                function submit() {

                    vm.properties.map(p => {
                        var value = p.value;
                        if (p.editor == "Umbraco.MultipleTextstring") {
                            value = p.value.map(v => v.value);
                        }
                        else if (p.editor == "Umbraco.MultiNodeTreePicker") {
                            value = p.value.split(',').map(v => parseInt(v));
                        }
                        else if (p.editor == "Umbraco.Integer") {
                            value = parseInt(p.value);
                        }
                        else if (p.editor == "Umbraco.Decimal") {
                            value = parseFloat(p.value);
                        }
                        else if (p.editor == "Umbraco.TrueFalse") {
                            value = p.value == "1";
                        }

                        $scope.model.advancedSettings[p.alias] = value;
                    });
                    if ($scope.model.submit) {
                        $scope.model.submit($scope.model);
                    }
                }

                function close() {
                    if ($scope.model.close) {
                        $scope.model.close();
                    }
                }
            }
        ]);
