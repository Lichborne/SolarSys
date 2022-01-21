pipeline {
    agent {
        docker { image 'unityci/editor:ubuntu-2020.3.25f1-webgl-0.16.1' 
//		args '-u root:sudo'
		args '-v /etc/passwd:/etc/passwd:ro'
            alwaysPull false
		
        }
    }
    stages {
        stage('Build') {
            steps {
                sh 'unity-editor  -batchmode -manualLicenseFile *.ulf -logfile lic_log.txt | true; cat lic_log.txt'
                sh 'unity-editor -quit -batchmode -projectPath ./ -executeMethod WebGLBuilder.build -logFile log.txt; ret=$?; cat log.txt; exit $ret'
            }
        }
    }
}
