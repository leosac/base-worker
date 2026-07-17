pipeline {
    agent any
    environment {
        DOTNET_SYSTEM_GLOBALIZATION_INVARIANT = 1
    }
    tools {
        dotnetsdk 'dotnet10'
    }
	stages {
        stage('Pre-Build') {
            steps {
                dotnetRestore()
            }
        }
	    stage('Build') {
            steps {
                dotnetBuild()
            }
        }
        stage('Sonar') {
            environment {
                scannerHome = tool 'SonarScanner for MSBuild'
            }
            steps {
                withSonarQubeEnv('SonarQube Community') {
                    sh "dotnet ${scannerHome}/SonarScanner.MSBuild.dll begin /k:'leosac_base-worker_e555830b-5ac4-482d-9d2c-33b2db819b66'"
                    dotnetBuild()
                    sh "dotnet ${scannerHome}/SonarScanner.MSBuild.dll end"
                }
                timeout(time: 1, unit: 'HOURS') {
                    waitForQualityGate(abortPipeline: true)
                }
            }
            when {
                anyOf {
                    branch 'main'
                    buildingTag()
                    changeRequest()
                }
            }
        }
    }
}