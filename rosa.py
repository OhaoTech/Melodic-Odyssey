from flask import Flask, request, jsonify
import librosa
import numpy as np

app = Flask(__name__)

@app.route('/upload', methods=['POST'])
def upload_file():
    if 'file' not in request.files:
        return "No file part"
    
    file = request.files['file']
    if file.filename == '':
        return "No selected file"
    
    if file:
        audio, sr = librosa.load(file, sr=None)
        pitches, magnitudes = librosa.piptrack(y=audio, sr=sr)
        
        # Extracting pitch values and their timestamps
        pitch_data = []
        for t in range(pitches.shape[1]):
            index = magnitudes[:, t].argmax()
            pitch = pitches[index, t]
            time = librosa.frames_to_time(t, sr=sr)
            pitch_data.append({'time': time, 'pitch': float(pitch)})  # Convert pitch to float

        # TODO: Map pitch values to musical notes

        # Create a JSON response
        response = {
            "pitch_data": pitch_data
        }

        return jsonify(response)

if __name__ == '__main__':
    app.run(debug=True)