pipeline {
    agent {
        docker { image 'unityci/editor:ubuntu-2020.3.25f1-webgl-0.16.1' 
		args '-u root:sudo'
//		args '-v /etc/passwd:/etc/passwd:ro'
 	        alwaysPull false	
        }
    }
    stages {
        stage('Build') {
            steps {
                sh 'unity-editor  -batchmode -manualLicenseFile *.ulf -logfile lic_log.txt | true; cat lic_log.txt'
                sh 'ret=0; unity-editor -quit -batchmode -projectPath ./ -executeMethod WebGLBuilder.build -logFile log.txt || ret=1 | true; cat log.txt; exit $ret'
		sh 'chown -R 117 ./; chgrp -R 122 ./'
		}
        }
    }
}
