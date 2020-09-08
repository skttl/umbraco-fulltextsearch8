# Github Workflow Actions

This project contains some basic github actions that will allow 
you to automate the build and deployment of your projects.

## build.yml 
This script will build and package your project, based on a 
`release` tag.

if you tag a commit with `release/(version)` (where version is the version you want e.g `release/1.0.0` ) then the build will produce a NuGet and umbraco package of that version.