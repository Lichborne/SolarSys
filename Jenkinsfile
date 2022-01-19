pipeline {
    agent {
        docker { image 'unityci/editor:ubuntu-2020.3.25f1-webgl-0.16.1' 
		args '-u root:sudo'
            alwaysPull false
		
        }
    }
    stages {
        stage('Build') {
            steps {
		sh 'whoami' 
                sh 'unity-editor  -batchmode -manualLicenseFile ./*.ulf -logfile'
                sh 'unity-editor -quit -batchmode -projectPath ./ -executeMethod WebGLBuilder.build -logFile log.txt'

            }
        }
    }
}
