import React, { useState } from "react";
import axios from "axios";

export const FileUpload = () => {
  const [file, setFile] = useState();
  const [fileName, setFileName] = useState();
  const [res1, setRes] = useState([]);
  const [loading, setLoading] = useState(false); 

  const saveFile = (e) => {
    console.log(e.target.files[0]);
    setFile(e.target.files[0]);
    setFileName(e.target.files[0].name);
  };

  const uploadFile = async (e) => {
    setLoading(true);
    console.log(file);
    const formData = new FormData();
    formData.append("formFile", file);
    formData.append("fileName", fileName);
    try {
      const res = await axios.post("https://localhost:8081/api/file", formData);
      setRes(res.data);
      console.log(res);
    } catch (ex) {
      console.log(ex);
    }
    finally {
      setLoading(false);
    }
  };

  return (
    <>
    <div>
      <input type="file" disabled={loading} onChange={saveFile} />
      <input type="button" disabled={loading} value="upload" onClick={uploadFile} /></div>
      <br/>
      {loading ? "batch operation is in progress"  : <div>
      {res1.map(function(d, idx){
         return (<li>{d.description}</li>)
       })}
      </div>}
    </>
  );
  
};
