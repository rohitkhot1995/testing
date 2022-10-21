node {
  stage('SCM') {
    checkout scm
  }
  stage('SonarQube Analysis') {
       
        def scannerHome = tool 'SonarScanner for MSBuild'
        withSonarQubeEnv('sonar') {
          
          sh "dotnet ${scannerHome}/SonarScanner.MSBuild.Common.dll begin /k:\"dotnet\""      
          sh "dotnet build Gaming.Predictor.sln"
          sh "dotnet ${scannerHome}/SonarScanner.MSBuild.Common.dll end"           
        }

    }
}

